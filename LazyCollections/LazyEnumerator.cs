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
            Current = _collectionEnumerator.Current;
            return true;
        }

        _readingFromCollection = false;
        bool ret = _original.MoveNext();
        Current = _original.Current;
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
        Current = _collectionEnumerator.Current;
    }

    /// <inheritdoc />
    public TContents? Current { get; private set; }

    /// <inheritdoc />
    object IEnumerator.Current => Current;

    /// <inheritdoc />
    public void Dispose()
    {
        _original.Dispose();
        _collectionEnumerator.Dispose();
    }

    public LazyEnumerator<TContents, TCollection> StartNew() => new(_original, Collection);
}