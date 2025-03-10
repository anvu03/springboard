using SpringBoard.Domain.Common;

namespace SpringBoard.Domain.Entities;

/// <summary>
/// Represents a refresh token used for authentication.
/// </summary>
public class RefreshToken : BaseEntity
{
    /// <summary>
    /// Gets or sets the token value.
    /// </summary>
    public required string Token { get; set; }
    
    /// <summary>
    /// Gets or sets the expiration date of the token.
    /// </summary>
    public required DateTime ExpiresAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date when the token was created.
    /// </summary>
    public required DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date when the token was revoked, if applicable.
    /// </summary>
    public DateTime? RevokedAt { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the token is active.
    /// </summary>
    public bool IsActive => RevokedAt == null && !IsExpired;
    
    /// <summary>
    /// Gets a value indicating whether the token has expired.
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    
    /// <summary>
    /// Gets or sets the ID of the user this token belongs to.
    /// </summary>
    public required Guid UserId { get; set; }
    
    /// <summary>
    /// Gets or sets the user this token belongs to.
    /// </summary>
    public User? User { get; set; }
}
