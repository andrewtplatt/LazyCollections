namespace LazyCollections;

/// <summary>
/// A lazily enumerated dictionary (hash map)
///
/// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}"/>
/// </summary>
public class LazyDictionary<TKey, TValue> : LazyCollection<KeyValuePair<TKey, TValue>, Dictionary<TKey, TValue>>, IReadOnlyDictionary<TKey, TValue> where TKey : notnull
{
    /// <summary>
    /// Constructor
    /// </summary>
    public LazyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> original) : base(original)
    {
    }

    /// <inheritdoc />
    public bool ContainsKey(TKey key)
    {
        if (UnderlyingCollection.ContainsKey(key))
        {
            return true;
        }

        while (EnumerateTo(CountEnumerated + 1))
        {
            if (UnderlyingCollection.ContainsKey(key))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public bool TryGetValue(TKey key, out TValue value)
    {
        if (UnderlyingCollection.TryGetValue(key, out value!))
        {
            return true;
        }

        while (EnumerateTo(CountEnumerated + 1))
        {
            if (UnderlyingCollection.TryGetValue(key, out value!))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public TValue this[TKey key]
    {
        get
        {
            if (UnderlyingCollection.TryGetValue(key, out var value))
            {
                return value;
            }

            while (EnumerateTo(CountEnumerated + 1))
            {
                if (UnderlyingCollection.TryGetValue(key, out value!))
                {
                    return value;
                }
            }

            throw new KeyNotFoundException($"Key {key} not found!");
        }
    }

    /// <inheritdoc />
    public IEnumerable<TKey> Keys => this.Select(kvp => kvp.Key);

    /// <inheritdoc />
    public IEnumerable<TValue> Values => this.Select(kvp => kvp.Value);
}