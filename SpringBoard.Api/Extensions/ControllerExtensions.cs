using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SpringBoard.Api.Extensions;

/// <summary>
/// Extensions for API controllers to simplify common operations.
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// Gets the current user's ID from claims.
    /// </summary>
    /// <param name="controller">The controller instance.</param>
    /// <returns>The user ID as Guid if found and valid; otherwise, null.</returns>
    public static Guid? GetCurrentUserId(this ControllerBase controller)
    {
        // Look for the sub claim which contains the user ID
        var userIdClaim = controller.User.FindFirst(ClaimTypes.NameIdentifier) ?? 
                          controller.User.FindFirst("sub");
        
        if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
        {
            return null;
        }

        // Try to parse the user ID as a Guid
        if (Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        return null;
    }

    /// <summary>
    /// Gets the current user's ID from claims or throws an exception if not found or invalid.
    /// </summary>
    /// <param name="controller">The controller instance.</param>
    /// <returns>The user ID as Guid.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when user ID is not found or invalid.</exception>
    public static Guid GetRequiredCurrentUserId(this ControllerBase controller)
    {
        var userId = controller.GetCurrentUserId();
        
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("User ID not found in claims or is invalid.");
        }
        
        return userId.Value;
    }
    
    /// <summary>
    /// Gets the current user's username from claims.
    /// </summary>
    /// <param name="controller">The controller instance.</param>
    /// <returns>The username if found; otherwise, null.</returns>
    public static string? GetCurrentUsername(this ControllerBase controller)
    {
        return controller.User.FindFirst("username")?.Value;
    }
    
    /// <summary>
    /// Gets the current user's email from claims.
    /// </summary>
    /// <param name="controller">The controller instance.</param>
    /// <returns>The email if found; otherwise, null.</returns>
    public static string? GetCurrentEmail(this ControllerBase controller)
    {
        return controller.User.FindFirst(ClaimTypes.Email)?.Value ?? 
               controller.User.FindFirst("email")?.Value;
    }
    
    /// <summary>
    /// Gets the current user's first name from claims.
    /// </summary>
    /// <param name="controller">The controller instance.</param>
    /// <returns>The first name if found; otherwise, null.</returns>
    public static string? GetCurrentFirstName(this ControllerBase controller)
    {
        return controller.User.FindFirst("firstName")?.Value;
    }
    
    /// <summary>
    /// Gets the current user's last name from claims.
    /// </summary>
    /// <param name="controller">The controller instance.</param>
    /// <returns>The last name if found; otherwise, null.</returns>
    public static string? GetCurrentLastName(this ControllerBase controller)
    {
        return controller.User.FindFirst("lastName")?.Value;
    }
    
    /// <summary>
    /// Gets the current user's full name from claims.
    /// </summary>
    /// <param name="controller">The controller instance.</param>
    /// <returns>The full name if first name or last name is found; otherwise, null.</returns>
    public static string? GetCurrentFullName(this ControllerBase controller)
    {
        var firstName = controller.GetCurrentFirstName();
        var lastName = controller.GetCurrentLastName();
        
        if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
        {
            return null;
        }
        
        return $"{firstName} {lastName}".Trim();
    }
    
    /// <summary>
    /// Gets all claims for the current user.
    /// </summary>
    /// <param name="controller">The controller instance.</param>
    /// <returns>A dictionary of claim types and values.</returns>
    public static IDictionary<string, string> GetCurrentUserClaims(this ControllerBase controller)
    {
        return controller.User.Claims
            .GroupBy(c => c.Type)
            .ToDictionary(
                group => group.Key,
                group => group.First().Value
            );
    }
    
    /// <summary>
    /// Gets a specific claim value for the current user.
    /// </summary>
    /// <param name="controller">The controller instance.</param>
    /// <param name="claimType">The type of claim to retrieve.</param>
    /// <returns>The claim value if found; otherwise, null.</returns>
    public static string? GetCurrentUserClaim(this ControllerBase controller, string claimType)
    {
        return controller.User.FindFirst(claimType)?.Value;
    }
}
