namespace SpringBoard.Domain.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found in the system.
/// </summary>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public class EntityNotFoundException<TId> : DomainException
{
    /// <summary>
    /// Gets the identifier of the entity that was not found.
    /// </summary>
    public TId Id { get; }

    /// <summary>
    /// Gets the name of the entity type that was not found.
    /// </summary>
    public string EntityName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException{TId}"/> class.
    /// </summary>
    /// <param name="entityName">The name of the entity type.</param>
    /// <param name="id">The identifier of the entity that was not found.</param>
    public EntityNotFoundException(string entityName, TId id)
        : base($"Entity '{entityName}' with id '{id}' was not found.")
    {
        EntityName = entityName;
        Id = id;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException{TId}"/> class with a custom message.
    /// </summary>
    /// <param name="entityName">The name of the entity type.</param>
    /// <param name="id">The identifier of the entity that was not found.</param>
    /// <param name="message">A custom error message.</param>
    public EntityNotFoundException(string entityName, TId id, string message)
        : base(message)
    {
        EntityName = entityName;
        Id = id;
    }
}

/// <summary>
/// Non-generic version of EntityNotFoundException for cases where the ID type is not known or needed.
/// </summary>
public class EntityNotFoundException : DomainException
{
    /// <summary>
    /// Gets the name of the entity type that was not found.
    /// </summary>
    public string EntityName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
    /// </summary>
    /// <param name="entityName">The name of the entity type.</param>
    public EntityNotFoundException(string entityName)
        : base($"Entity '{entityName}' was not found.")
    {
        EntityName = entityName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class with a custom message.
    /// </summary>
    /// <param name="entityName">The name of the entity type.</param>
    /// <param name="message">A custom error message.</param>
    public EntityNotFoundException(string entityName, string message)
        : base(message)
    {
        EntityName = entityName;
    }
}