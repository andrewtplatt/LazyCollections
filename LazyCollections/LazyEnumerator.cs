using System.Collections;

namespace LazyCollections;

/// <summary>
/// Lazy enumerator class.
/// </summary>
/// <typeparam name="TContents">The type of the contents to enumerate</typeparam>
/// <typeparam name="TCollection">The underlying backing store for the enumerated contents</typeparam>
internal class LazyEnumerator<TContents, TCollection> : IEnumerator<TContents> where TCollection : ICollection<TContents>, new()
{
    private readonly IEnumerator<TContents> _collectionEnumerator;
    private readonly IEnumerator<TContents> _original;

    /// <summary>
    /// Provides access to the already-enumerated items
    /// </summary>
    internal TCollection Collection { get; }

    /// <summary>
    /// Denotes whether or not the original enumerator has been fully exhausted
    /// </summary>
    internal bool FullyEnumerated { get; private set; }

    private bool _readingFromCollection;

    /// <summary>
    /// Constructor from a source enumerator
    /// </summary>
    public LazyEnumerator(IEnumerator<TContents> original) : this(original, new TCollection())
    { }

    private LazyEnumerator(IEnumerator<TContents> original, TCollection cache)
    {
        _original = original;
        Collection = cache;
        _collectionEnumerator = Collection.GetEnumerator();
        _readingFromCollection = true;
    }

    /// <inheritdoc />
    public bool MoveNext()
    {
        if (_readingFromCollection && _collectionEnumerator.MoveNext())
        {
            return true;
        }

        _readingFromCollection = false;
        bool ret = _original.MoveNext();
        if (ret)
        {
            Collection.Add(Current);
        }
        else
        {
            FullyEnumerated = true;
        }

        return ret;
    }

    /// <inheritdoc />
    public void Reset()
    {
        _collectionEnumerator.Reset();
        _readingFromCollection = true;
    }

    /// <inheritdoc />
    public TContents Current => _readingFromCollection ? _collectionEnumerator.Current : _original.Current;

    /// <inheritdoc />
    object IEnumerator.Current => Current!; // Suppress the warning - we want the return from this to be identical to the above

    /// <inheritdoc />
    public void Dispose()
    {
        _original.Dispose();
        _collectionEnumerator.Dispose();
    }

    public LazyEnumerator<TContents, TCollection> StartNew() => new(_original, Collection);
}