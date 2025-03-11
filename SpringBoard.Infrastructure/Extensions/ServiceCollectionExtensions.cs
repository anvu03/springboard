using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpringBoard.Application.Abstractions.Persistence.Repositories;
using SpringBoard.Application.Abstractions.Services;
using SpringBoard.Infrastructure.Options;
using SpringBoard.Infrastructure.Persistence.Repositories;
using SpringBoard.Infrastructure.Services;
using System.Text;

namespace SpringBoard.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all infrastructure services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register JWT options
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        
        // Register token service
        services.AddScoped<ITokenService, TokenService>();
        
        // Register current user service
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        // Register repositories and unit of work
        services.AddRepositoryServices();
        
        // Register services with [Service] attribute
        services.AddServices();
        
        // Add JWT authentication
        services.AddJwtAuthentication(configuration);
        
        return services;
    }

    /// <summary>
    /// Adds persistence services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        return services;
    }

    /// <summary>
    /// Adds repository services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<IUserRepository, FakeUserRepository>();
        services.AddScoped<IRefreshTokenRepository, FakeRefreshTokenRepository>();
        
        // Note: FakeUnitOfWork is automatically registered via the [Service] attribute
        
        return services;
    }

    /// <summary>
    /// Adds external services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddExternalServices(this IServiceCollection services)
    {
        return services;
    }
    
    /// <summary>
    /// Adds JWT authentication to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // Use IOptions pattern to access JWT settings
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
                RequireSignedTokens = true
            };
            
            // Configure JWT Bearer with options from IOptions<JwtOptions>
            options.RequireHttpsMetadata = false; // Set to true in production
            options.SaveToken = true;
            
            // Use provider to resolve options at runtime
            var serviceProvider = services.BuildServiceProvider();
            var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;
            
            // Set the validation parameters from the options
            var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);
            options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(key);
            options.TokenValidationParameters.ValidIssuer = jwtOptions.Issuer;
            options.TokenValidationParameters.ValidAudience = jwtOptions.Audience;
        });

        return services;
    }
}
