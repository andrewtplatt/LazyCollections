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
        if (enumerable is LazyHashSet<T> lhs)
        {
            return lhs;
        }
        return new LazyHashSet<T>(enumerable);
    }
    
    /// <summary>
    /// Create a new <see cref="LazyList{T}"/> from the given enumerable.
    /// </summary>
    /// <param name="enumerable">The enumerable to enumerate lazily</param>
    /// <typeparam name="T">The type of enumerable contents</typeparam>
    /// <returns>A new instance of <see cref="LazyList{T}"/></returns>
    public static LazyList<T> ToLazyList<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable is LazyList<T> lhs)
        {
            return lhs;
        }
        return new LazyList<T>(enumerable);
    }

    /// <summary>
    /// Create a new <see cref="LazyDictionary{TKey,TValue}"/> from the given enumerable.
    /// </summary>
    /// <param name="enumerable">The enumerable to enumerate lazily</param>
    /// <returns>A new instance of <see cref="LazyDictionary{TKey,TValue}"/></returns>
    public static LazyDictionary<TKey, TValue> ToLazyDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
        where TKey : notnull
    {
        if (enumerable is LazyDictionary<TKey, TValue> lhs)
        {
            return lhs;
        }
        return new LazyDictionary<TKey, TValue>(enumerable);
    }

    /// <summary>
    /// Create a new <see cref="LazyDictionary{TKey,TValue}"/> from the given enumerable.
    /// </summary>
    /// <param name="enumerable">The enumerable to enumerate lazily</param>
    /// <param name="keySelector">Function to create keys from the enumerable's values</param>
    /// <param name="valueSelector">Function to create values from the enumerable's values</param>
    /// <returns>A new instance of <see cref="LazyDictionary{TKey,TValue}"/></returns>
    public static LazyDictionary<TKey, TValue> ToLazyDictionary<T, TKey, TValue>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector, Func<T, TValue> valueSelector)
        where TKey : notnull
    {
        return new LazyDictionary<TKey, TValue>(enumerable.Select(x => KeyValuePair.Create(keySelector(x), valueSelector(x))));
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