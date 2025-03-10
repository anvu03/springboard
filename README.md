# SpringBoard

A modern, clean architecture ASP.NET Core Web API template for building secure and scalable applications.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)
![License](https://img.shields.io/badge/license-MIT-blue)

## 🚀 Features

- **Clean Architecture** - Follows the principles of Clean Architecture with clear separation of concerns
- **Domain-Driven Design** - Implements DDD patterns for complex business logic
- **JWT Authentication** - Secure authentication with JWT tokens and refresh token rotation
- **CQRS with MediatR** - Command Query Responsibility Segregation pattern using MediatR
- **Global Exception Handling** - Standardized exception handling with ProblemDetails (RFC 7807)
- **Swagger/OpenAPI** - API documentation with Swagger UI and JWT authentication support
- **Repository Pattern** - Abstraction over data access with repository and unit of work patterns
- **Dependency Injection** - Uses attribute-based DI registration with Devlooped.Extensions.DependencyInjection.Attributed

## 🏗️ Project Structure

```
SpringBoard/
├── SpringBoard.Api/              # API layer - Controllers, Middleware, Configuration
├── SpringBoard.Application/      # Application layer - Commands, Queries, Interfaces
├── SpringBoard.Domain/           # Domain layer - Entities, Value Objects, Domain Exceptions
└── SpringBoard.Infrastructure/   # Infrastructure layer - Repository Implementations, External Services
```

### Layers

- **Domain Layer**: Contains enterprise logic and types
- **Application Layer**: Contains business logic and types
- **Infrastructure Layer**: Contains all external concerns
- **API Layer**: Contains everything related to ASP.NET Core Web API

## 🔒 Authentication

SpringBoard implements a secure authentication system using JWT tokens:

- JWT token-based authentication
- Refresh token rotation for enhanced security
- Password hashing with ASP.NET Core Identity's PasswordHasher
- Login with either username or email

## 🛡️ Exception Handling

The application uses a domain exception hierarchy for business rule violations:

- `DomainException`: Base exception for all domain-specific errors
- `EntityNotFoundException`: Thrown when an entity cannot be found
- `DomainUnauthorizedException`: Thrown for authorization violations
- `InvalidEntityStateException`: Thrown when an entity is in an invalid state
- `DuplicateEntityException`: Thrown when attempting to create a duplicate entity

All exceptions are handled globally and mapped to appropriate HTTP status codes with standardized ProblemDetails responses.

## 🚦 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)

### Renaming the Project

The template includes a script that allows you to easily rename the solution and all projects to match your desired project name:

1. Make the script executable (if needed)
   ```bash
   chmod +x rename-project.sh
   ```

2. Run the script with your desired project name
   ```bash
   ./rename-project.sh TestProject YourProjectName
   ```

3. The script will:
   - Rename all project directories and files
   - Update all namespaces and references in code files
   - Update the solution file
   - Skip binary files and files listed in .gitignore

4. Build the solution to verify all references are updated correctly
   ```bash
   dotnet build
   ```

> **Note:** After renaming, you may need to reload your IDE to see all changes. Also, remember to manually check any custom build scripts, Docker files, or CI/CD configurations.

### Running the Application

1. Clone the repository
   ```bash
   git clone https://github.com/yourusername/testproject.git
   cd testproject
   ```

2. Build the solution
   ```bash
   dotnet build
   ```

3. Run the API
   ```bash
   cd SpringBoard.Api
   dotnet run
   ```

4. Open your browser and navigate to:
   ```
   https://localhost:5001/swagger
   ```

## 🧪 Testing the API

1. Register a new user using the `/api/auth/register` endpoint
2. Login with the registered user credentials using the `/api/auth/login` endpoint
3. Use the returned JWT token in the Authorization header for subsequent requests
4. When the token expires, use the refresh token to get a new JWT token

## 🛠️ Customization

### Adding a New Entity

1. Create a new entity class in the `SpringBoard.Domain/Entities` folder
2. Create a repository interface in the `SpringBoard.Application/Interfaces` folder
3. Implement the repository in the `SpringBoard.Infrastructure/Persistence/Repositories` folder
4. Register the repository in the DI container

### Adding a New Endpoint

1. Create Command/Query classes in the `SpringBoard.Application/Features` folder
2. Implement the Command/Query handlers
3. Create a controller in the `SpringBoard.Api/Controllers` folder
4. Inject MediatR and send the Command/Query

## 📚 Technologies

- [ASP.NET Core 9](https://docs.microsoft.com/en-us/aspnet/core)
- [MediatR](https://github.com/jbogard/MediatR)
- [JWT Bearer Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication)
- [Swagger/OpenAPI](https://swagger.io)
- [Devlooped.Extensions.DependencyInjection.Attributed](https://github.com/devlooped/extensions.dependencyinjection.attributed)
- [Riok.Mapperly](https://github.com/riok/mapperly)

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.
