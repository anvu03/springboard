using System.ComponentModel.DataAnnotations;

namespace SpringBoard.Api.Models.Auth;

/// <summary>
/// API request model for user login.
/// </summary>
public record LoginRequest
{
    /// <summary>
    /// Gets or sets the username or email for authentication.
    /// </summary>
    [Required]
    [StringLength(100)]
    public required string UsernameOrEmail { get; init; }

    /// <summary>
    /// Gets or sets the password for authentication.
    /// </summary>
    [Required]
    [StringLength(100)]
    public required string Password { get; init; }
}
