using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace LazyCollections;

/// <summary>
/// A lazily enumerated collection with underlying collection type <see cref="LinkedList{T}"/>.
/// </summary>
public class LazyLinkedList<T> : ILazyCollection<T>
{
    private readonly LazyLinkedListEnumerator<T> _enumerator;
    internal LazyLinkedListNode<T>? _first;
    internal LazyLinkedListNode<T>? _last;

    /// <inheritdoc />
    public LazyLinkedList(IEnumerable<T> original)
    {
        _enumerator = new LazyLinkedListEnumerator<T>(original.GetEnumerator(), this);
    }

    /// <summary>
    /// Gets the first node of the <see cref="T:LazyCollections.LazyLinkedList`1" />.
    /// </summary>
    /// <returns>
    /// The first <see cref="T:LazyCollections.LazyLinkedListNode`1" /> of the <see cref="T:LazyCollections.LazyLinkedList`1" />.
    /// </returns>
    public LazyLinkedListNode<T>? First
    {
        get
        {
            EnumerateTo(1); // this should set _first
            return _first;
        }
    }

    /// <summary>
    /// Gets the last node of the <see cref="T:LazyCollections.LazyLinkedList`1" />.
    /// </summary>
    /// <returns>
    /// The last <see cref="T:LazyCollections.LazyLinkedListNode`1" /> of the <see cref="T:LazyCollections.LazyLinkedList`1" />.
    /// </returns>
    public LazyLinkedListNode<T>? Last
    {
        get
        {
            EnumerateTo(int.MaxValue); // this should set _last
            return _last;
        }
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => _enumerator.StartNew();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
    public int CountEnumerated => _enumerator.CountEnumerated;

    /// <inheritdoc />
    public bool FullyEnumerated => _enumerator.FullyEnumerated;

    /// <summary>
    /// Enumerate up to the first i elements of the original enumerable. If the original enumerable yielded fewer than
    /// i elements, stop when the original enumerable is exhausted.
    /// </summary>
    /// <param name="i">The number of elements to enumerate to</param>
    /// <returns>Whether or not any additional elements were sourced from the original enumerable</returns>
    private bool EnumerateTo(int i)
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
    /// Enumerate up to the next i elements of the original enumerable. If the original enumerable yielded fewer than
    /// i elements, stop when the original enumerable is exhausted.
    /// </summary>
    /// <param name="i">The number of elements to enumerate to</param>
    /// <returns>Whether or not any additional elements were sourced from the original enumerable</returns>
    internal bool AdvanceStream(int i)
    {
        bool ret = false;
        for (int j = 0; j < i && !FullyEnumerated; j++)
        {
            _enumerator.MoveNext();
            ret = true;
        }

        return ret;
    }
}

/// <summary>
/// Linked list node for a <see cref="LazyLinkedList{T}"/>
/// </summary>
public sealed class LazyLinkedListNode<T>
{
    internal LazyLinkedListNode<T>? _prev;
    internal LazyLinkedListNode<T>? _next;
    private bool _advanced;

    internal LazyLinkedListNode(T value, LazyLinkedList<T> list)
    {
        ArgumentNullException.ThrowIfNull(list);
        Value = value;
        List = list;
    }

    /// <summary>
    /// The lazy linked list this node is part of
    /// </summary>
    public LazyLinkedList<T> List { get; }

    /// <summary>Gets the next node in the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
    /// <returns>A reference to the next node in the <see cref="T:System.Collections.Generic.LinkedList`1" />, or <see langword="null" /> if the current node is the last element (<see cref="P:System.Collections.Generic.LinkedList`1.Last" />) of the <see cref="T:System.Collections.Generic.LinkedList`1" />.</returns>
    public LazyLinkedListNode<T>? Next
    {
        get
        {
            if (!_advanced && _next is null)
            {
                _advanced = true;
                List.AdvanceStream(1);
            }

            return _next;
        }
    }

    /// <summary>Gets the previous node in the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
    /// <returns>A reference to the previous node in the <see cref="T:System.Collections.Generic.LinkedList`1" />, or <see langword="null" /> if the current node is the first element (<see cref="P:System.Collections.Generic.LinkedList`1.First" />) of the <see cref="T:System.Collections.Generic.LinkedList`1" />.</returns>
    public LazyLinkedListNode<T>? Previous => _prev;

    /// <summary>Gets the value contained in the node.</summary>
    /// <returns>The value contained in the node.</returns>
    public T Value { get; }
}

/// <summary>
/// Lazy enumerator class.
/// </summary>
/// <typeparam name="T">The type of the contents to enumerate</typeparam>
internal sealed class LazyLinkedListEnumerator<T> : IEnumerator<T>
{
    [Flags]
    private enum State
    {
        FullyEnumerated = 1 << 0,
        AtEnd = 1 << 1,
    }

    private readonly CountingEnumerator<T> _original;
    private readonly LazyLinkedList<T> _list;
    private State _state;

    internal LazyLinkedListNode<T>? CurrentNode { get; private set; }

    /// <summary>
    /// Denotes whether or not the original enumerator has been fully exhausted
    /// </summary>
    internal bool FullyEnumerated => _state != 0;

    internal int CountEnumerated => _original.Count;

    /// <summary>
    /// Constructor from a source enumerator
    /// </summary>
    public LazyLinkedListEnumerator(IEnumerator<T> original, LazyLinkedList<T> list) : this(new CountingEnumerator<T>(original), list)
    { }

    private LazyLinkedListEnumerator(CountingEnumerator<T> original, LazyLinkedList<T> list)
    {
        _original = original;
        _list = list;
    }

    /// <inheritdoc />
    public bool MoveNext()
    {
        if ((_state & State.AtEnd) != 0)
        {
            Debug.Assert(CurrentNode is null);
            return false;
        }

        if (CountEnumerated == 0)
        {
            if (_original.MoveNext())
            {
                var n = new LazyLinkedListNode<T>(_original.Current, _list);
                _list._first = n;
                CurrentNode = n;
                return true;
            }

            _state |= State.FullyEnumerated;
            return false;
        }

        if (CurrentNode is null)
        {
            // We have enumerated the original stream already and reset to the start
            Debug.Assert(_list._first is not null);
            CurrentNode = _list._first;
            return true;
        }

        if (CurrentNode._next is not null)
        {
            CurrentNode = CurrentNode._next;
            return true;
        }

        if (_original.MoveNext())
        {
            var n = new LazyLinkedListNode<T>(_original.Current, _list);
            CurrentNode._next = n;
            n._prev = CurrentNode;
            CurrentNode = n;
            return true;
        }

        _list._last = CurrentNode;
        CurrentNode = null;
        _state |= State.FullyEnumerated;

        return false;
    }

    /// <inheritdoc />
    public void Reset()
    {
        CurrentNode = null;
        _state &= ~State.AtEnd;
    }

    #nullable disable
    /// <inheritdoc />
    public T Current => CurrentNode is null ? default : CurrentNode.Value;
    #nullable restore

    /// <inheritdoc />
    object IEnumerator.Current => Current!; // Suppress the warning - we want the return from this to be identical to the above

    /// <inheritdoc />
    public void Dispose()
    {
        _original.Dispose();
    }

    public LazyLinkedListEnumerator<T> StartNew() => new(_original, _list);
}
