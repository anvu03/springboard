namespace SpringBoard.Application.Features.Auth.Models;

/// <summary>
/// Response model for authentication operations.
/// </summary>
public record AuthResponse
{
    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public required Guid UserId { get; init; }
    
    /// <summary>
    /// Gets or sets the JWT token.
    /// </summary>
    public required string IdToken { get; init; }
    
    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    public required string RefreshToken { get; init; }
}
