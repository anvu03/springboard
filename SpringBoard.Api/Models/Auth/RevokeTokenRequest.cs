using System.ComponentModel.DataAnnotations;

namespace SpringBoard.Api.Models.Auth;

/// <summary>
/// API request model for revoking a specific refresh token.
/// </summary>
public record RevokeTokenRequest
{
    /// <summary>
    /// Gets or sets the token to revoke.
    /// </summary>
    [Required]
    public required string Token { get; init; }
}
