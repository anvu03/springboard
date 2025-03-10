using SpringBoard.Domain.Entities;

namespace SpringBoard.Application.Abstractions.Persistence.Repositories;

/// <summary>
/// Interface for refresh token repository operations.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Creates a new refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to create.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateAsync(RefreshToken refreshToken);
    
    /// <summary>
    /// Gets a refresh token by its token value.
    /// </summary>
    /// <param name="token">The token value.</param>
    /// <returns>The refresh token if found; otherwise, null.</returns>
    Task<RefreshToken?> GetByTokenAsync(string token);
    
    /// <summary>
    /// Gets all active refresh tokens for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>A collection of active refresh tokens.</returns>
    Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Revokes a refresh token.
    /// </summary>
    /// <param name="token">The token value to revoke.</param>
    /// <param name="revokedAt">The date when the token was revoked.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RevokeAsync(string token, DateTime revokedAt);
    
    /// <summary>
    /// Revokes all refresh tokens for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="revokedAt">The date when the tokens were revoked.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RevokeAllForUserAsync(Guid userId, DateTime revokedAt);
}
