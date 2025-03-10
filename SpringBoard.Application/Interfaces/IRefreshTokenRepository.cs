using SpringBoard.Domain.Entities;

namespace SpringBoard.Application.Interfaces;

/// <summary>
/// Repository interface for managing refresh tokens
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Creates a new refresh token
    /// </summary>
    /// <param name="refreshToken">The refresh token to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a refresh token by its token string
    /// </summary>
    /// <param name="token">The token string to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The refresh token entity if found, null otherwise</returns>
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Revokes a refresh token
    /// </summary>
    /// <param name="refreshToken">The refresh token to revoke</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RevokeAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all active refresh tokens for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of active refresh tokens for the user</returns>
    Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Revokes all refresh tokens for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
