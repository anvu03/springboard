namespace SpringBoard.Domain.Exceptions;

/// <summary>
/// Exception thrown when a user attempts to perform an action they are not authorized to do.
/// </summary>
public class DomainUnauthorizedException : DomainException
{
    /// <summary>
    /// Gets the name of the resource that was attempted to be accessed.
    /// </summary>
    public string? Resource { get; }

    /// <summary>
    /// Gets the type of action that was attempted.
    /// </summary>
    public string? Action { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainUnauthorizedException"/> class.
    /// </summary>
    public DomainUnauthorizedException()
        : base("You are not authorized to perform this action.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainUnauthorizedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DomainUnauthorizedException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainUnauthorizedException"/> class with a specified resource and action.
    /// </summary>
    /// <param name="resource">The resource that was attempted to be accessed.</param>
    /// <param name="action">The action that was attempted.</param>
    public DomainUnauthorizedException(string resource, string action)
        : base($"You are not authorized to {action} the {resource}.")
    {
        Resource = resource;
        Action = action;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainUnauthorizedException"/> class with a specified resource, action, and custom message.
    /// </summary>
    /// <param name="resource">The resource that was attempted to be accessed.</param>
    /// <param name="action">The action that was attempted.</param>
    /// <param name="message">A custom error message.</param>
    public DomainUnauthorizedException(string resource, string action, string message)
        : base(message)
    {
        Resource = resource;
        Action = action;
    }
}