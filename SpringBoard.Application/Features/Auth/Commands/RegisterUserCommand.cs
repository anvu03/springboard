using MediatR;
using Microsoft.AspNetCore.Identity;
using SpringBoard.Application.Abstractions.Persistence;
using SpringBoard.Domain.Entities;
using SpringBoard.Domain.Exceptions;

namespace SpringBoard.Application.Features.Auth.Commands;

/// <summary>
/// Command for registering a new user in the system.
/// </summary>
public record RegisterUserCommand : IRequest<Guid>
{
    /// <summary>
    /// Gets or sets the username for the new user.
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// Gets or sets the email address for the new user.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Gets or sets the password for the new user.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    /// Gets or sets the first name of the new user. This is optional.
    /// </summary>
    public string? FirstName { get; init; }

    /// <summary>
    /// Gets or sets the last name of the new user. This is optional.
    /// </summary>
    public string? LastName { get; init; }
}

/// <summary>
/// Handler for processing the <see cref="RegisterUserCommand"/>.
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PasswordHasher<User> _passwordHasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    public RegisterUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = new PasswordHasher<User>();
    }

    /// <summary>
    /// Handles the registration of a new user.
    /// </summary>
    /// <param name="request">The registration command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The ID of the newly created user.</returns>
    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Validate username doesn't contain email-like characters
        if (request.Username.Contains('@') || request.Username.Contains('.'))
        {
            throw new InvalidEntityStateException("User", "Username must not contain email-like characters (@ or .)");
        }

        // Check if username already exists
        if (await _unitOfWork.Users.ExistsByUsernameAsync(request.Username))
        {
            throw new DuplicateEntityException("User", "Username", request.Username);
        }

        // Check if email already exists
        if (await _unitOfWork.Users.ExistsByEmailAsync(request.Email))
        {
            throw new DuplicateEntityException("User", "Email", request.Email);
        }

        // Validate password strength
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            throw new InvalidEntityStateException("User", "Password must be at least 8 characters long");
        }

        // Create a temporary user for password hashing
        var tempUser = new User
        {
            Username = request.Username.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = "temp" // Temporary value that will be immediately replaced
        };
        
        // Hash the password using ASP.NET Core Identity's PasswordHasher
        string hashedPassword = _passwordHasher.HashPassword(tempUser, request.Password);
        
        // Create the actual user entity with all required properties
        var user = new User
        {
            Username = request.Username.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = hashedPassword,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        // Begin transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Save the user
            await _unitOfWork.Users.CreateAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync();

            return user.Id;
        }
        catch
        {
            // Rollback transaction if an error occurs
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}