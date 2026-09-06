using System.Collections;

namespace LazyCollections;

internal class CountingEnumerator<T> : IEnumerator<T>
{
    private readonly IEnumerator<T> _original;

    public CountingEnumerator(IEnumerator<T> original)
    {
        _original = original;
    }

    public int Count { get; private set; }
    
    public bool MoveNext()
    {
        if (_original.MoveNext())
        {
            Count++;
            return true;
        }

        return false;
    }

    public void Reset()
    {
        _original.Reset();
        Count = 0;
    }

    public T Current => _original.Current;

    object? IEnumerator.Current => Current;

    public void Dispose()
    {
        _original.Dispose();
    }
}