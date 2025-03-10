namespace SpringBoard.Domain.Exceptions;

/// <summary>
/// Exception thrown when an entity is in an invalid state for a requested operation.
/// </summary>
public class InvalidEntityStateException : DomainException
{
    /// <summary>
    /// Gets the name of the entity that is in an invalid state.
    /// </summary>
    public string? EntityName { get; }

    /// <summary>
    /// Gets the identifier of the entity that is in an invalid state.
    /// </summary>
    public object? EntityId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidEntityStateException"/> class.
    /// </summary>
    public InvalidEntityStateException()
        : base("The entity is in an invalid state for this operation.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidEntityStateException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InvalidEntityStateException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidEntityStateException"/> class with a specified entity name.
    /// </summary>
    /// <param name="entityName">The name of the entity that is in an invalid state.</param>
    /// <param name="useEntityName">A dummy parameter to differentiate from the message constructor.</param>
    public InvalidEntityStateException(string entityName, bool useEntityName)
        : base($"The {entityName} is in an invalid state for this operation.")
    {
        EntityName = entityName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidEntityStateException"/> class with a specified entity name and ID.
    /// </summary>
    /// <param name="entityName">The name of the entity that is in an invalid state.</param>
    /// <param name="entityId">The identifier of the entity that is in an invalid state.</param>
    public InvalidEntityStateException(string entityName, object entityId)
        : base($"The {entityName} with ID '{entityId}' is in an invalid state for this operation.")
    {
        EntityName = entityName;
        EntityId = entityId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidEntityStateException"/> class with a specified entity name, ID, and custom message.
    /// </summary>
    /// <param name="entityName">The name of the entity that is in an invalid state.</param>
    /// <param name="entityId">The identifier of the entity that is in an invalid state.</param>
    /// <param name="message">A custom error message.</param>
    public InvalidEntityStateException(string entityName, object entityId, string message)
        : base(message)
    {
        EntityName = entityName;
        EntityId = entityId;
    }


}