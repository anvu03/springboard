namespace SpringBoard.Application.DTOs;

/// <summary>
/// Response DTO for authentication operations
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// JWT token for API authorization
    /// </summary>
    public required string Token { get; set; }
    
    /// <summary>
    /// Refresh token for obtaining a new JWT token when it expires
    /// </summary>
    public required string RefreshToken { get; set; }
    
    /// <summary>
    /// The authenticated user's ID
    /// </summary>
    public required Guid UserId { get; set; }
}
