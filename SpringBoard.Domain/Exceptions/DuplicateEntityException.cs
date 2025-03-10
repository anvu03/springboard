namespace SpringBoard.Domain.Exceptions;

/// <summary>
/// Exception thrown when an attempt is made to create an entity that already exists.
/// </summary>
public class DuplicateEntityException : DomainException
{
    /// <summary>
    /// Gets the name of the entity that already exists.
    /// </summary>
    public string? EntityName { get; }

    /// <summary>
    /// Gets the identifier or key value that caused the duplicate conflict.
    /// </summary>
    public object? DuplicateKey { get; }

    /// <summary>
    /// Gets the name of the property that caused the duplicate conflict.
    /// </summary>
    public string? PropertyName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateEntityException"/> class.
    /// </summary>
    public DuplicateEntityException()
        : base("An entity with the same key already exists.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateEntityException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DuplicateEntityException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateEntityException"/> class with a specified entity name.
    /// </summary>
    /// <param name="entityName">The name of the entity that already exists.</param>
    /// <param name="useEntityName">A dummy parameter to differentiate from the message constructor.</param>
    public DuplicateEntityException(string entityName, bool useEntityName)
        : base($"A {entityName} with the same key already exists.")
    {
        EntityName = entityName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateEntityException"/> class with a specified entity name and duplicate key.
    /// </summary>
    /// <param name="entityName">The name of the entity that already exists.</param>
    /// <param name="duplicateKey">The identifier or key value that caused the duplicate conflict.</param>
    public DuplicateEntityException(string entityName, object duplicateKey)
        : base($"A {entityName} with the key '{duplicateKey}' already exists.")
    {
        EntityName = entityName;
        DuplicateKey = duplicateKey;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateEntityException"/> class with a specified entity name, property name, and duplicate key.
    /// </summary>
    /// <param name="entityName">The name of the entity that already exists.</param>
    /// <param name="propertyName">The name of the property that caused the duplicate conflict.</param>
    /// <param name="duplicateKey">The identifier or key value that caused the duplicate conflict.</param>
    public DuplicateEntityException(string entityName, string propertyName, object duplicateKey)
        : base($"A {entityName} with the same {propertyName} '{duplicateKey}' already exists.")
    {
        EntityName = entityName;
        PropertyName = propertyName;
        DuplicateKey = duplicateKey;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateEntityException"/> class with a specified entity name, property name, duplicate key, and custom message.
    /// </summary>
    /// <param name="entityName">The name of the entity that already exists.</param>
    /// <param name="propertyName">The name of the property that caused the duplicate conflict.</param>
    /// <param name="duplicateKey">The identifier or key value that caused the duplicate conflict.</param>
    /// <param name="message">A custom error message.</param>
    public DuplicateEntityException(string entityName, string propertyName, object duplicateKey, string message)
        : base(message)
    {
        EntityName = entityName;
        PropertyName = propertyName;
        DuplicateKey = duplicateKey;
    }
}