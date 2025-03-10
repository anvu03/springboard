namespace SpringBoard.Application.Abstractions.Services;

/// <summary>
/// Service for accessing information about the current authenticated user.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's ID.
    /// </summary>
    /// <returns>The user ID if authenticated; otherwise, null.</returns>
    Guid? GetUserId();
    
    /// <summary>
    /// Gets the current user's ID or throws an exception if not authenticated.
    /// </summary>
    /// <returns>The user ID.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when user is not authenticated or ID is invalid.</exception>
    Guid GetRequiredUserId();
    
    /// <summary>
    /// Gets the current user's username.
    /// </summary>
    /// <returns>The username if available; otherwise, null.</returns>
    string? GetUsername();
    
    /// <summary>
    /// Gets the current user's email.
    /// </summary>
    /// <returns>The email if available; otherwise, null.</returns>
    string? GetEmail();
    
    /// <summary>
    /// Gets the current user's first name.
    /// </summary>
    /// <returns>The first name if available; otherwise, null.</returns>
    string? GetFirstName();
    
    /// <summary>
    /// Gets the current user's last name.
    /// </summary>
    /// <returns>The last name if available; otherwise, null.</returns>
    string? GetLastName();
    
    /// <summary>
    /// Gets the current user's full name.
    /// </summary>
    /// <returns>The full name if available; otherwise, null.</returns>
    string? GetFullName();
    
    /// <summary>
    /// Gets a specific claim value for the current user.
    /// </summary>
    /// <param name="claimType">The type of claim to retrieve.</param>
    /// <returns>The claim value if found; otherwise, null.</returns>
    string? GetClaim(string claimType);
    
    /// <summary>
    /// Gets all claims for the current user.
    /// </summary>
    /// <returns>A dictionary of claim types and values.</returns>
    IDictionary<string, string> GetAllClaims();
    
    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    /// <returns>True if the user is authenticated; otherwise, false.</returns>
    bool IsAuthenticated();
}
