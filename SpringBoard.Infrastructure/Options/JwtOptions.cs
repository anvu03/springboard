using System;

namespace SpringBoard.Infrastructure.Options;

/// <summary>
/// Options for JWT token configuration.
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// The section name in the configuration.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Gets or sets the secret key used to sign JWT tokens.
    /// </summary>
    public required string SecretKey { get; set; }

    /// <summary>
    /// Gets or sets the issuer of the JWT tokens.
    /// </summary>
    public required string Issuer { get; set; }

    /// <summary>
    /// Gets or sets the audience of the JWT tokens.
    /// </summary>
    public required string Audience { get; set; }

    /// <summary>
    /// Gets or sets the token expiration time span.
    /// Default is 1 hour if not specified.
    /// </summary>
    public TimeSpan TokenExpiration { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Gets or sets the refresh token expiration time span.
    /// Default is 7 days if not specified.
    /// </summary>
    public TimeSpan RefreshTokenExpiration { get; set; } = TimeSpan.FromDays(7);
}
