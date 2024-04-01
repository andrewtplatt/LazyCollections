namespace LazyCollections;

/// <summary>
/// A lazily enumerated collection with underlying collection type <see cref="List{T}"/>.
///
/// <inheritdoc cref="IReadOnlyList{T}"/>
/// </summary>
public class LazyList<T> : LazyCollection<T, List<T>>, IReadOnlyList<T>
{
    /// <inheritdoc />
    public LazyList(IEnumerable<T> original) : base(original)
    { }

    /// <inheritdoc />
    /// <remarks>This enumerates the original enumerable up to the requested index</remarks>
    public T this[int index]
    {
        get
        {
            EnumerateTo(index);
            return UnderlyingCollection[index];
        }
    }
}