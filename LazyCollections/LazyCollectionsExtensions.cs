namespace LazyCollections;

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