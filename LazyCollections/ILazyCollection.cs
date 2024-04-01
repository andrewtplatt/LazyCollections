namespace LazyCollections;

/// <summary>
/// Lazily enumerated collection.
///
/// <inheritdoc cref="IReadOnlyCollection{T}"/>
/// </summary>
/// <typeparam name="T">The type of the items in the collection.</typeparam>
public interface ILazyCollection<out T> : IReadOnlyCollection<T>
{
    /// <summary>
    /// The number of items in the collection that have been enumerated. If the underlying collection removes items
    /// e.g. duplicates, these will not be included here.
    /// </summary>
    int CountEnumerated { get; }

    /// <summary>
    /// Indicates whether the original enumerable has been fully enumerated or not.
    /// </summary>
    bool FullyEnumerated { get; }
}