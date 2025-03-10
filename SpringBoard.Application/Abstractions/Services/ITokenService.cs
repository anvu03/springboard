using SpringBoard.Domain.Entities;

namespace SpringBoard.Application.Abstractions.Services;

/// <summary>
/// Interface for token generation and validation services.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token for a user.
    /// </summary>
    /// <param name="user">The user to generate the token for.</param>
    /// <returns>The generated JWT token.</returns>
    string GenerateJwtToken(User user);

    /// <summary>
    /// Generates a refresh token for a user.
    /// </summary>
    /// <param name="userId">The ID of the user to generate the token for.</param>
    /// <returns>The generated refresh token entity.</returns>
    Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId);

    /// <summary>
    /// Validates a JWT token.
    /// </summary>
    /// <param name="token">The token to validate.</param>
    /// <returns>True if the token is valid; otherwise, false.</returns>
    bool ValidateJwtToken(string token);

    /// <summary>
    /// Gets the user ID from a JWT token.
    /// </summary>
    /// <param name="token">The JWT token.</param>
    /// <returns>The user ID if the token is valid; otherwise, null.</returns>
    Guid? GetUserIdFromToken(string token);
}
