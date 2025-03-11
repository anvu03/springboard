using MediatR;
using SpringBoard.Application.Abstractions.Persistence;
using SpringBoard.Application.Abstractions.Services;
using SpringBoard.Application.Features.Auth.Models;
using SpringBoard.Domain.Exceptions;
using System.Security.Cryptography;

namespace SpringBoard.Application.Features.Auth.Commands;

/// <summary>
/// Command for refreshing an authentication token.
/// </summary>
public record RefreshTokenCommand : IRequest<AuthResponse>
{
    /// <summary>
    /// Gets or sets the refresh token to validate.
    /// </summary>
    public required string RefreshToken { get; init; }
}

/// <summary>
/// Handler for processing the <see cref="RefreshTokenCommand"/>.
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="tokenService">The token service.</param>
    public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Handles the refresh token validation and generation of a new token.
    /// </summary>
    /// <param name="request">The refresh token command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Authentication response containing user ID, ID token, and refresh token.</returns>
    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new DomainUnauthorizedException("Invalid refresh token");
        }

        // Find the refresh token
        var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);

        // Validate the token
        if (refreshToken == null || !refreshToken.IsActive)
        {
            // Add a small random delay to prevent timing attacks
            RandomDelay();
            throw new DomainUnauthorizedException("Invalid or expired refresh token");
        }

        // Get the user associated with the token
        var user = await _unitOfWork.Users.GetByIdAsync(refreshToken.UserId);
        if (user == null || !user.IsActive)
        {
            // Add a small random delay to prevent timing attacks
            RandomDelay();
            throw new DomainUnauthorizedException("User account is deactivated or does not exist");
        }

        // Begin transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Revoke the current token
            await _unitOfWork.RefreshTokens.RevokeAsync(refreshToken.Token, DateTime.UtcNow);

            // Generate a new refresh token
            var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);
            await _unitOfWork.RefreshTokens.CreateAsync(newRefreshToken);

            // Generate a new JWT token
            string idToken = _tokenService.GenerateJwtToken(user);

            // Update the user's last login timestamp
            await _unitOfWork.Users.UpdateLastLoginAsync(user.Id, DateTime.UtcNow);
            
            // Save changes and commit the transaction
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync();

            return new AuthResponse
            {
                UserId = user.Id,
                IdToken = idToken,
                RefreshToken = newRefreshToken.Token
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