using MediatR;
using Microsoft.AspNetCore.Identity;
using SpringBoard.Application.Abstractions.Persistence;
using SpringBoard.Application.Abstractions.Services;
using SpringBoard.Application.Features.Auth.Models;
using SpringBoard.Domain.Entities;
using SpringBoard.Domain.Exceptions;
using System.Security.Cryptography;

namespace SpringBoard.Application.Features.Auth.Commands;

/// <summary>
/// Command for authenticating a user.
/// </summary>
public record LoginCommand : IRequest<AuthResponse>
{
    /// <summary>
    /// Gets or sets the username or email for authentication.
    /// </summary>
    public required string UsernameOrEmail { get; init; }

    /// <summary>
    /// Gets or sets the password for authentication.
    /// </summary>
    public required string Password { get; init; }
}

/// <summary>
/// Handler for processing the <see cref="LoginCommand"/>.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="tokenService">The token service.</param>
    public LoginCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = new PasswordHasher<User>();
        _tokenService = tokenService;
    }

    /// <summary>
    /// Handles the authentication of a user.
    /// </summary>
    /// <param name="request">The login command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Authentication response containing user ID, ID token, and refresh token.</returns>
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.UsernameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new DomainUnauthorizedException("Invalid username/email or password");
        }

        // Normalize input
        string usernameOrEmail = request.UsernameOrEmail.Trim();

        // Implement constant-time lookup to prevent timing attacks
        // Always perform both lookups regardless of input type to maintain constant time
        var userByEmail = usernameOrEmail.Contains('@') ?
            await _unitOfWork.Users.GetByEmailAsync(usernameOrEmail.ToLowerInvariant()) : null;

        var userByUsername = await _unitOfWork.Users.GetByUsernameAsync(usernameOrEmail);

        // Select the appropriate user based on the input type
        User? user = usernameOrEmail.Contains('@') ? userByEmail : userByUsername;

        // If user not found, perform a dummy password check to prevent timing attacks
        if (user == null)
        {
            // Create a dummy user and perform password verification to maintain constant time
            var dummyUser = new User
            {
                Username = "dummy",
                Email = "dummy@example.com",
                PasswordHash = "AQAAAAIAAYagAAAAELELsT8jeJjDPbcP/x0z7EUq9A6wQZ9mrhjxf/Bpxs4PEfKGJUZzk2/RBbYKF8vP8g=="
            };
            _passwordHasher.VerifyHashedPassword(dummyUser, dummyUser.PasswordHash, request.Password);

            // Add a small random delay to further mitigate timing attacks
            RandomDelay();

            throw new DomainUnauthorizedException("Invalid username/email or password");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            // Still perform password verification to maintain constant time
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            // Add a small random delay to further mitigate timing attacks
            RandomDelay();

            throw new DomainUnauthorizedException("Account is deactivated");
        }

        // Verify password
        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            // Add a small random delay to further mitigate timing attacks
            RandomDelay();

            throw new DomainUnauthorizedException("Invalid username/email or password");
        }

        // If password needs rehash (SuccessRehashNeeded), update it
        if (passwordVerificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            // Update password hash with a newer algorithm or work factor
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
        }

        // Begin transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Update last login timestamp
            await _unitOfWork.Users.UpdateLastLoginAsync(user.Id, DateTime.UtcNow);

            // Generate JWT token
            string idToken = _tokenService.GenerateJwtToken(user);

            // Generate refresh token
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

            // Save the refresh token
            await _unitOfWork.RefreshTokens.CreateAsync(refreshToken);

            // Save changes and commit the transaction
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync();

            return new AuthResponse
            {
                UserId = user.Id,
                IdToken = idToken,
                RefreshToken = refreshToken.Token
            };
        }
        catch
        {
            // Rollback transaction if an error occurs
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    /// <summary>
    /// Adds a small random delay to mitigate timing attacks.
    /// </summary>
    private static void RandomDelay()
    {
        // Generate a random delay between 100-300ms
        int delayMs = RandomNumberGenerator.GetInt32(100, 300);
        Thread.Sleep(delayMs);
    }
}