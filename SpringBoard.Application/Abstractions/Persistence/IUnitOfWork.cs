using SpringBoard.Application.Abstractions.Persistence.Repositories;

namespace SpringBoard.Application.Abstractions.Persistence;

/// <summary>
/// Defines the Unit of Work pattern interface for managing transactions and repository access.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Gets the user repository.
    /// </summary>
    IUserRepository Users { get; }
    
    /// <summary>
    /// Gets the refresh token repository.
    /// </summary>
    IRefreshTokenRepository RefreshTokens { get; }

    /// <summary>
    /// Saves all changes made in this unit of work to the database.
    /// </summary>
    /// <returns>The number of affected records.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CommitTransactionAsync();

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RollbackTransactionAsync();
}