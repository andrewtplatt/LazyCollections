using System.Collections;

namespace LazyCollections;

/// <summary>
/// Base class for lazily evaluated collections.
/// </summary>
/// <typeparam name="TContents">The type of the contents of the collection</typeparam>
/// <typeparam name="TCollection">The type of the underlying backing store for the collection</typeparam>
public abstract class LazyCollection<TContents, TCollection> : ILazyCollection<TContents> 
    where TCollection : ICollection<TContents>, new()
{
    private readonly LazyEnumerator<TContents, TCollection> _enumerator;

    /// <summary>
    /// Constructor given the original enumerable to enumerate
    /// </summary>
    /// <param name="original"></param>
    protected LazyCollection(IEnumerable<TContents> original)
    {
        _enumerator = new LazyEnumerator<TContents, TCollection>(original.GetEnumerator());
    }

    /// <inheritdoc />
    public IEnumerator<TContents> GetEnumerator() => _enumerator.StartNew();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Enumerate up to the first i elements of the original enumerable. If the original enumerable yielded fewer than
    /// i elements, stop when the original enumerable is exhausted.
    /// </summary>
    /// <param name="i">The number of elements to enumerate to</param>
    /// <returns>Whether or not any additional elements were sourced from the original enumerable</returns>
    protected bool EnumerateTo(int i)
    {
        bool ret = false;
        while (!FullyEnumerated && CountEnumerated < i)
        {
            _enumerator.MoveNext();
            ret = true;
        }

        return ret;
    }

    /// <summary>
    /// Provides access to the underlying collection
    /// </summary>
    protected TCollection UnderlyingCollection => _enumerator.Collection;

    /// <inheritdoc />
    /// <remarks>This fully enumerates the collection</remarks>
    public int Count
    {
        get
        {
            EnumerateTo(int.MaxValue);
            return CountEnumerated;
        }
    }

    /// <inheritdoc />
    public int CountEnumerated => _enumerator.Collection.Count;

    /// <inheritdoc />
    public bool FullyEnumerated => _enumerator.FullyEnumerated;
}