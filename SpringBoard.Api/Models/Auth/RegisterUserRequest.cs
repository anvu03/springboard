using System.ComponentModel.DataAnnotations;
using SpringBoard.Api.Validation;

namespace SpringBoard.Api.Models.Auth;

/// <summary>
/// API request model for user registration.
/// </summary>
public record RegisterUserRequest
{
    /// <summary>
    /// Gets or sets the username for the new user.
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 3)]
    [NoEmailCharacters]
    public required string Username { get; init; }

    /// <summary>
    /// Gets or sets the email address for the new user.
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public required string Email { get; init; }

    /// <summary>
    /// Gets or sets the password for the new user.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 8)]
    public required string Password { get; init; }

    /// <summary>
    /// Gets or sets the first name of the new user. This is optional.
    /// </summary>
    [StringLength(50)]
    public string? FirstName { get; init; }

    /// <summary>
    /// Gets or sets the last name of the new user. This is optional.
    /// </summary>
    [StringLength(50)]
    public string? LastName { get; init; }
}
