using SpringBoard.Api.Conventions;
using SpringBoard.Api.Extensions;
using SpringBoard.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    // Add kebab-case routing convention for controllers
    options.Conventions.Add(new KebabCaseControllerRouteConvention());
});

// Register services with [Service] attribute
builder.Services.AddServices();

// Register MediatR services
builder.Services.AddMediatR(cfg => {
    // Register handlers from Application layer
    cfg.RegisterServicesFromAssembly(typeof(SpringBoard.Application.Features.Auth.Commands.LoginCommand).Assembly);
});

// Configure infrastructure services and authentication
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddAuthorizationPolicies();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SpringBoard API",
        Version = "v1",
        Description = "API for the SpringBoard application",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "SpringBoard Team",
            Email = "support@springboard.example.com"
        }
    });

    // Configure JWT authentication in Swagger UI
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Add global exception handler middleware
app.UseGlobalExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SpringBoard API v1");
    c.RoutePrefix = "swagger";
});

// Set Swagger as the default page
app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
