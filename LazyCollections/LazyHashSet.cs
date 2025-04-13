namespace LazyCollections;

/// <summary>
/// A lazily enumerated collection with underlying collection type <see cref="HashSet{T}"/>.
///
/// <inheritdoc cref="IReadOnlySet{T}"/>
/// </summary>
public class LazyHashSet<T> : LazyCollection<T, HashSet<T>>, IReadOnlySet<T>
{
    /// <inheritdoc />
    public LazyHashSet(IEnumerable<T> original) : base(original)
    { }

    /// <inheritdoc />
    /// <remarks>This will enumerate the underlying enumerable until the requested item is found</remarks>
    public bool Contains(T item)
    {
        if (UnderlyingCollection.Contains(item))
        {
            return true;
        }

        while (EnumerateTo(CountEnumerated + 1))
        {
            if (UnderlyingCollection.Contains(item))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        var other2 = other.ToLazyHashSet();
        return other2.IsProperSupersetOf(this);
    }

    /// <inheritdoc />
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        int ct = 0;
        foreach (var t in other.Distinct())
        {
            if (!Contains(t))
            {
                return false;
            }

            ct++;
        }

        return ct < Count;
    }

    /// <inheritdoc />
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        var other2 = other.ToLazyHashSet();
        return other2.IsProperSupersetOf(this);
    }

    /// <inheritdoc />
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        foreach (var t in other)
        {
            if (!Contains(t))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public bool Overlaps(IEnumerable<T> other)
    {
        foreach (var t in other)
        {
            if (Contains(t))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public bool SetEquals(IEnumerable<T> other)
    {
        var lh = other.ToLazyHashSet();
        return IsSupersetOf(lh) && IsSubsetOf(lh);
    }
}