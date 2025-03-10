using SpringBoard.Domain.Entities;

namespace SpringBoard.Application.Abstractions.Persistence.Repositories;

/// <summary>
/// Repository interface for user-related data access operations.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Creates a new user in the repository.
    /// </summary>
    /// <param name="user">The user entity to create.</param>
    /// <returns>The created user with assigned ID.</returns>
    Task<User> CreateAsync(User user);

    /// <summary>
    /// Updates an existing user in the repository.
    /// </summary>
    /// <param name="user">The user entity to update.</param>
    /// <returns>The updated user.</returns>
    Task<User> UpdateAsync(User user);

    /// <summary>
    /// Deletes a user from the repository.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>True if the user was deleted, false otherwise.</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Gets a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The user if found, null otherwise.</returns>
    Task<User?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets a user by their username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <returns>The user if found, null otherwise.</returns>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary>
    /// Gets a user by their email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <returns>The user if found, null otherwise.</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Checks if a username already exists in the repository.
    /// </summary>
    /// <param name="username">The username to check.</param>
    /// <returns>True if the username exists, false otherwise.</returns>
    Task<bool> ExistsByUsernameAsync(string username);

    /// <summary>
    /// Checks if an email address already exists in the repository.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <returns>True if the email exists, false otherwise.</returns>
    Task<bool> ExistsByEmailAsync(string email);

    /// <summary>
    /// Updates the last login timestamp for a user.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <param name="loginTime">The login timestamp.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateLastLoginAsync(Guid id, DateTime loginTime);
}