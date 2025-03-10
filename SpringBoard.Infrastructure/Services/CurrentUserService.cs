using Microsoft.AspNetCore.Http;
using SpringBoard.Application.Abstractions.Services;
using System.Security.Claims;

namespace SpringBoard.Infrastructure.Services;

/// <summary>
/// Implementation of the current user service that uses HttpContext to access user claims.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserService"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc/>
    public Guid? GetUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier) ?? 
                          _httpContextAccessor.HttpContext?.User.FindFirst("sub");
        
        if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
        {
            return null;
        }

        if (Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        return null;
    }

    /// <inheritdoc/>
    public Guid GetRequiredUserId()
    {
        var userId = GetUserId();
        
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("User ID not found in claims or is invalid.");
        }
        
        return userId.Value;
    }

    /// <inheritdoc/>
    public string? GetUsername()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst("username")?.Value;
    }

    /// <inheritdoc/>
    public string? GetEmail()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value ?? 
               _httpContextAccessor.HttpContext?.User.FindFirst("email")?.Value;
    }

    /// <inheritdoc/>
    public string? GetFirstName()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst("firstName")?.Value;
    }

    /// <inheritdoc/>
    public string? GetLastName()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst("lastName")?.Value;
    }

    /// <inheritdoc/>
    public string? GetFullName()
    {
        var firstName = GetFirstName();
        var lastName = GetLastName();
        
        if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
        {
            return null;
        }
        
        return $"{firstName} {lastName}".Trim();
    }

    /// <inheritdoc/>
    public string? GetClaim(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst(claimType)?.Value;
    }

    /// <inheritdoc/>
    public IDictionary<string, string> GetAllClaims()
    {
        var claims = _httpContextAccessor.HttpContext?.User.Claims;
        
        if (claims == null)
        {
            return new Dictionary<string, string>();
        }
        
        return claims
            .GroupBy(c => c.Type)
            .ToDictionary(
                group => group.Key,
                group => group.First().Value
            );
    }

    /// <inheritdoc/>
    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }
}
