using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpringBoard.Infrastructure.Options;
using System.Text;

namespace SpringBoard.Api.Extensions;

/// <summary>
/// Extensions for setting up authentication in the API.
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    /// Adds JWT bearer authentication to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // Configure JWT Bearer with options from IOptions<JwtOptions>
            var serviceProvider = services.BuildServiceProvider();
            var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;
            
            var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);
            
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
                RequireSignedTokens = true
            };
            
            // In production, set this to true
            options.RequireHttpsMetadata = false;
            
            // Configure events for additional validation or handling
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        context.Response.Headers.Append("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    // Additional validation can be performed here if needed
                    return Task.CompletedTask;
                }
            };
        });
        
        return services;
    }
    
    /// <summary>
    /// Adds authorization policies to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Define policies based on claims or roles
            options.AddPolicy("RequireAdminRole", policy => 
                policy.RequireClaim("role", "admin"));
                
            options.AddPolicy("RequireUserRole", policy => 
                policy.RequireClaim("role", "user"));
                
            // Add more policies as needed
        });
        
        return services;
    }
}
