using System.ComponentModel.DataAnnotations;

namespace SpringBoard.Api.Models.Auth;

/// <summary>
/// API request model for refreshing an authentication token.
/// </summary>
public record RefreshTokenRequest
{
    /// <summary>
    /// Gets or sets the refresh token to validate.
    /// </summary>
    [Required]
    public required string RefreshToken { get; init; }
}
