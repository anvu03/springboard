using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpringBoard.Application.Abstractions.Persistence;
using SpringBoard.Application.Abstractions.Persistence.Repositories;

namespace SpringBoard.Infrastructure.Persistence;

/// <summary>
/// An in-memory implementation of IUnitOfWork for testing and development purposes.
/// </summary>
[Service(ServiceLifetime.Scoped)]
public class FakeUnitOfWork : IUnitOfWork
{
    private readonly ILogger<FakeUnitOfWork> _logger;
    private bool _isTransactionActive = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeUnitOfWork"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="refreshTokenRepository">The refresh token repository.</param>
    /// <param name="logger">The logger.</param>
    public FakeUnitOfWork(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<FakeUnitOfWork> logger)
    {
        Users = userRepository;
        RefreshTokens = refreshTokenRepository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public IUserRepository Users { get; }

    /// <inheritdoc/>
    public IRefreshTokenRepository RefreshTokens { get; }

    /// <inheritdoc/>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // In a real implementation, this would save changes to a database
        _logger.LogInformation("Saving changes to the database");
        return Task.FromResult(1); // Return 1 to indicate success
    }

    /// <inheritdoc/>
    public Task BeginTransactionAsync()
    {
        if (_isTransactionActive)
        {
            throw new InvalidOperationException("A transaction is already active");
        }

        _isTransactionActive = true;
        _logger.LogInformation("Beginning transaction");
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task CommitTransactionAsync()
    {
        if (!_isTransactionActive)
        {
            throw new InvalidOperationException("No active transaction to commit");
        }

        _isTransactionActive = false;
        _logger.LogInformation("Committing transaction");
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task RollbackTransactionAsync()
    {
        if (!_isTransactionActive)
        {
            throw new InvalidOperationException("No active transaction to rollback");
        }

        _isTransactionActive = false;
        _logger.LogInformation("Rolling back transaction");
        return Task.CompletedTask;
    }
}
