using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpringBoard.Application.Abstractions.Persistence;
using SpringBoard.Application.Abstractions.Services;
using SpringBoard.Domain.Entities;
using SpringBoard.Infrastructure.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SpringBoard.Infrastructure.Services;

/// <summary>
/// Implementation of the token service for generating and validating JWT tokens.
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TokenService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenService"/> class.
    /// </summary>
    /// <param name="jwtOptions">The JWT options.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="logger">The logger.</param>
    public TokenService(IOptions<JwtOptions> jwtOptions, IUnitOfWork unitOfWork, ILogger<TokenService> logger)
    {
        _jwtOptions = jwtOptions.Value;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <inheritdoc/>
    public string GenerateJwtToken(User user)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
            
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("username", user.Username),
                new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            // Add optional claims if available
            if (!string.IsNullOrEmpty(user.FirstName))
            {
                claims.Add(new Claim("firstName", user.FirstName));
            }
            
            if (!string.IsNullOrEmpty(user.LastName))
            {
                claims.Add(new Claim("lastName", user.LastName));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtOptions.TokenExpiration),
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                NotBefore = DateTime.UtcNow // Token is not valid before now
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating JWT token for user {UserId}", user.Id);
            throw new InvalidOperationException("Could not generate authentication token", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId)
    {
        try
        {
            // Generate a cryptographically secure random token
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            // Check if user exists before generating token
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"Cannot generate refresh token for non-existent user {userId}");
            }

            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(_jwtOptions.RefreshTokenExpiration)
            };

            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating refresh token for user {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc/>
    public bool ValidateJwtToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
                RequireSignedTokens = true
            }, out SecurityToken validatedToken);

            // Additional check: ensure token is a JWT token
            if (!(validatedToken is JwtSecurityToken jwtToken))
            {
                _logger.LogWarning("Invalid token type detected");
                return false;
            }

            return true;
        }
        catch (SecurityTokenExpiredException ex)
        {
            _logger.LogInformation(ex, "Token validation failed: Token expired");
            return false;
        }
        catch (SecurityTokenInvalidSignatureException ex)
        {
            _logger.LogWarning(ex, "Token validation failed: Invalid signature");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return false;
        }
    }

    /// <inheritdoc/>
    public Guid? GetUserIdFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
                RequireSignedTokens = true
            }, out SecurityToken validatedToken);

            if (!(validatedToken is JwtSecurityToken jwtToken))
            {
                _logger.LogWarning("Invalid token type detected when extracting user ID");
                return null;
            }

            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null)
            {
                _logger.LogWarning("Token does not contain user ID claim");
                return null;
            }

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                _logger.LogWarning("User ID in token is not a valid GUID");
                return null;
            }

            // Check if user still exists and is active
            var user = _unitOfWork.Users.GetByIdAsync(userId).GetAwaiter().GetResult();
            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("Token contains ID of non-existent or inactive user: {UserId}", userId);
                return null;
            }

            return userId;
        }
        catch (SecurityTokenExpiredException ex)
        {
            _logger.LogInformation(ex, "Token validation failed when extracting user ID: Token expired");
            return null;
        }
        catch (SecurityTokenInvalidSignatureException ex)
        {
            _logger.LogWarning(ex, "Token validation failed when extracting user ID: Invalid signature");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed when extracting user ID");
            return null;
        }
    }
}
