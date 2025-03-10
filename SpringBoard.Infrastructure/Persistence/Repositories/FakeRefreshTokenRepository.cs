using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpringBoard.Application.Abstractions.Persistence.Repositories;
using SpringBoard.Domain.Entities;
using SpringBoard.Domain.Exceptions;

namespace SpringBoard.Infrastructure.Persistence.Repositories;

/// <summary>
/// An in-memory implementation of IRefreshTokenRepository for testing and development purposes.
/// </summary>
[Service(ServiceLifetime.Singleton)]
public class FakeRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly List<RefreshToken> _refreshTokens = new();
    private readonly ILogger<FakeRefreshTokenRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeRefreshTokenRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public FakeRefreshTokenRepository(ILogger<FakeRefreshTokenRepository> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task CreateAsync(RefreshToken refreshToken)
    {
        if (refreshToken == null)
        {
            throw new ArgumentNullException(nameof(refreshToken));
        }

        // Generate a new ID if not provided
        if (refreshToken.Id == Guid.Empty)
        {
            refreshToken.Id = Guid.NewGuid();
        }

        _refreshTokens.Add(refreshToken);
        _logger.LogInformation("Created refresh token with ID {TokenId} for user {UserId}", 
            refreshToken.Id, refreshToken.UserId);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<RefreshToken?> GetByTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be null or whitespace", nameof(token));
        }

        var refreshToken = _refreshTokens.FirstOrDefault(t => t.Token == token);
        return Task.FromResult(refreshToken);
    }

    /// <inheritdoc/>
    public Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(Guid userId)
    {
        var activeTokens = _refreshTokens
            .Where(t => t.UserId == userId && t.IsActive)
            .ToList();

        return Task.FromResult<IEnumerable<RefreshToken>>(activeTokens);
    }

    /// <inheritdoc/>
    public Task RevokeAsync(string token, DateTime revokedAt)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be null or whitespace", nameof(token));
        }

        var refreshToken = _refreshTokens.FirstOrDefault(t => t.Token == token);
        if (refreshToken == null)
        {
            throw new EntityNotFoundException("Refresh token", token);
        }

        refreshToken.RevokedAt = revokedAt;
        _logger.LogInformation("Revoked refresh token with ID {TokenId} for user {UserId}", 
            refreshToken.Id, refreshToken.UserId);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task RevokeAllForUserAsync(Guid userId, DateTime revokedAt)
    {
        var activeTokens = _refreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToList();

        foreach (var token in activeTokens)
        {
            token.RevokedAt = revokedAt;
        }

        _logger.LogInformation("Revoked {Count} refresh tokens for user {UserId}", 
            activeTokens.Count, userId);

        return Task.CompletedTask;
    }
}
