using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpringBoard.Application.Abstractions.Persistence.Repositories;
using SpringBoard.Domain.Entities;
using SpringBoard.Domain.Exceptions;

namespace SpringBoard.Infrastructure.Persistence.Repositories;

/// <summary>
/// An in-memory implementation of IUserRepository for testing and development purposes.
/// </summary>
[Service(ServiceLifetime.Singleton)]
public class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    private readonly ILogger<FakeUserRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeUserRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public FakeUserRepository(ILogger<FakeUserRepository> logger)
    {
        _logger = logger;
        SeedInitialData();
    }

    /// <inheritdoc/>
    public Task<User> CreateAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        // Check for duplicate username
        if (_users.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
        {
            throw new DuplicateEntityException($"User with username '{user.Username}' already exists");
        }

        // Check for duplicate email
        if (_users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
        {
            throw new DuplicateEntityException($"User with email '{user.Email}' already exists");
        }

        // Generate a new ID if not provided
        if (user.Id == Guid.Empty)
        {
            user.Id = Guid.NewGuid();
        }

        // Set creation timestamp if not provided
        if (user.CreatedAt == default)
        {
            user.CreatedAt = DateTime.UtcNow;
        }

        _users.Add(user);
        _logger.LogInformation("Created user with ID {UserId}", user.Id);

        return Task.FromResult(user);
    }

    /// <inheritdoc/>
    public Task<User> UpdateAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser == null)
        {
            throw new EntityNotFoundException<Guid>("User", user.Id);
        }

        // Check for duplicate username (excluding the current user)
        if (_users.Any(u => u.Id != user.Id && u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
        {
            throw new DuplicateEntityException($"User with username '{user.Username}' already exists");
        }

        // Check for duplicate email (excluding the current user)
        if (_users.Any(u => u.Id != user.Id && u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
        {
            throw new DuplicateEntityException($"User with email '{user.Email}' already exists");
        }

        // Update user properties
        var index = _users.FindIndex(u => u.Id == user.Id);
        _users[index] = user;

        _logger.LogInformation("Updated user with ID {UserId}", user.Id);
        return Task.FromResult(user);
    }

    /// <inheritdoc/>
    public Task<bool> DeleteAsync(Guid id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            _logger.LogWarning("Attempted to delete non-existent user with ID {UserId}", id);
            return Task.FromResult(false);
        }

        _users.Remove(user);
        _logger.LogInformation("Deleted user with ID {UserId}", id);
        return Task.FromResult(true);
    }

    /// <inheritdoc/>
    public Task<User?> GetByIdAsync(Guid id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user);
    }

    /// <inheritdoc/>
    public Task<User?> GetByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be null or whitespace", nameof(username));
        }

        var user = _users.FirstOrDefault(u => 
            u.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    /// <inheritdoc/>
    public Task<User?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or whitespace", nameof(email));
        }

        var user = _users.FirstOrDefault(u => 
            u.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    /// <inheritdoc/>
    public Task<bool> ExistsByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be null or whitespace", nameof(username));
        }

        var exists = _users.Any(u => 
            u.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    /// <inheritdoc/>
    public Task<bool> ExistsByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or whitespace", nameof(email));
        }

        var exists = _users.Any(u => 
            u.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    /// <inheritdoc/>
    public Task UpdateLastLoginAsync(Guid id, DateTime loginTime)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            throw new EntityNotFoundException<Guid>("User", id);
        }

        user.LastLoginAt = loginTime;
        _logger.LogInformation("Updated last login time for user with ID {UserId}", id);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Seeds the repository with initial test data.
    /// </summary>
    private void SeedInitialData()
    {
        // Add a default admin user if the repository is empty
        if (!_users.Any())
        {
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = "AQAAAAIAAYagAAAAELTsZ76VmYXXTEiRCzxhw2fBcbYZQZIQHGLOVPmsc8KpMfKGXf3YZLNX9p8z5kWfmA==", // Password: Admin123!
                FirstName = "System",
                LastName = "Administrator",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _users.Add(adminUser);
            _logger.LogInformation("Seeded admin user with ID {UserId}", adminUser.Id);

            // Add a regular test user
            var testUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "AQAAAAIAAYagAAAAEOZRYpzGdPPZmrL4kl6Dq5QDJXH9VP4RpZJQXgD8YiuT/LNkf1y+BWyXnGUZoXqkrw==", // Password: Test123!
                FirstName = "Test",
                LastName = "User",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _users.Add(testUser);
            _logger.LogInformation("Seeded test user with ID {UserId}", testUser.Id);
        }
    }
}