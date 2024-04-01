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
    public IEnumerator<TContents> GetEnumerator() => _enumerator.Clone();

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

/// <summary>
/// Utility methods for working with Lazy Collections.
/// </summary>
public static class LazyCollection
{
    /// <summary>
    /// Create a new <see cref="LazyHashSet{T}"/> from the given enumerable.
    /// </summary>
    /// <param name="enumerable">The enumerable to enumerate lazily</param>
    /// <typeparam name="T">The type of enumerable contents</typeparam>
    /// <returns>A new instance of <see cref="LazyHashSet{T}"/></returns>
    public static LazyHashSet<T> ToLazyHashSet<T>(this IEnumerable<T> enumerable)
    {
        return new LazyHashSet<T>(enumerable);
    }

    /// <summary>
    /// Like <see cref="ToLazyHashSet{T}"/>, but if the enumerable is already a lazy hash set it does not create a new
    /// one.
    /// </summary>
    /// <param name="enumerable">The enumerable to enumerate lazily</param>
    /// <typeparam name="T">The type of enumerable contents</typeparam>
    /// <returns>A new or existing instance of <see cref="LazyHashSet{T}"/></returns>
    public static LazyHashSet<T> AsLazyHashSet<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable is LazyHashSet<T> lhs)
        {
            return lhs;
        }
        return new LazyHashSet<T>(enumerable);
    }
    
    /// <summary>
    /// Create a new <see cref="LazyHashSet{T}"/> from the given enumerable.
    /// </summary>
    /// <param name="enumerable">The enumerable to enumerate lazily</param>
    /// <typeparam name="T">The type of enumerable contents</typeparam>
    /// <returns>A new instance of <see cref="LazyList{T}"/></returns>
    public static LazyList<T> ToLazyList<T>(this IEnumerable<T> enumerable)
    {
        return new LazyList<T>(enumerable);
    }

    /// <summary>
    /// Like <see cref="ToLazyList{T}"/>, but if the enumerable is already a lazy list it does not create a new
    /// one.
    /// </summary>
    /// <param name="enumerable">The enumerable to enumerate lazily</param>
    /// <typeparam name="T">The type of enumerable contents</typeparam>
    /// <returns>A new or existing instance of <see cref="LazyList{T}"/></returns>
    public static LazyList<T> AsLazyList<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable is LazyList<T> lhs)
        {
            return lhs;
        }
        return new LazyList<T>(enumerable);
    }

    /// <summary>
    /// Enforce that an enumerable will be enumerated at most once in a lazy fashion.
    /// </summary>
    /// <param name="enumerable">The enumerable to enumerate lazily</param>
    /// <typeparam name="T">The type of enumerable contents</typeparam>
    /// <returns>A new or existing instance of <see cref="ILazyCollection{T}"/></returns>
    public static ILazyCollection<T> EnumerateOnlyOnce<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable is ILazyCollection<T> lhs)
        {
            return lhs;
        }
        return new LazyList<T>(enumerable);
    }
}