namespace LazyCollections.Tests;

[TestFixture]
[TestOf(typeof(LazyDictionary<,>))]
public class LazyDictionaryTests
{
    [Test]
    public void LazyCollection_EnumeratesIdenticallyToInputEnumerable()
    {
        var input = new Dictionary<int, int>();
        for (int i = 0; i < 10; i++)
        {
            input[Random.Shared.Next()] = Random.Shared.Next();
        }

        var lazy = input.ToLazyDictionary();

        using var groundTruth = input.GetEnumerator();
        using var lazily = lazy.GetEnumerator();
        while (groundTruth.MoveNext())
        {
            Assert.That(lazily.MoveNext());
            Assert.That(groundTruth.Current, Is.EqualTo(lazily.Current));
        }
        Assert.That(!lazily.MoveNext());
        Assert.That(lazy.ToList(), Is.EqualTo(input.ToList()));
    }

    [Test]
    public void KeysCollection_EnumeratesIdenticallyToInputEnumerable()
    {
        var input = new Dictionary<int, int>();
        for (int i = 0; i < 10; i++)
        {
            input[Random.Shared.Next()] = Random.Shared.Next();
        }

        var lazy = input.ToLazyDictionary();

        using var groundTruth = input.Keys.GetEnumerator();
        using var lazily = lazy.Keys.GetEnumerator();
        while (groundTruth.MoveNext())
        {
            Assert.That(lazily.MoveNext());
            Assert.That(groundTruth.Current, Is.EqualTo(lazily.Current));
        }
        Assert.That(!lazily.MoveNext());
        Assert.That(lazy.ToList(), Is.EqualTo(input.ToList()));
    }
    
    [Test]
    public void ValuesCollection_EnumeratesIdenticallyToInputEnumerable()
    {
        var input = new Dictionary<int, int>();
        for (int i = 0; i < 10; i++)
        {
            input[Random.Shared.Next()] = Random.Shared.Next();
        }

        var lazy = input.ToLazyDictionary();

        using var groundTruth = input.Values.GetEnumerator();
        using var lazily = lazy.Values.GetEnumerator();
        while (groundTruth.MoveNext())
        {
            Assert.That(lazily.MoveNext());
            Assert.That(groundTruth.Current, Is.EqualTo(lazily.Current));
        }
        Assert.That(!lazily.MoveNext());
        Assert.That(lazy.ToList(), Is.EqualTo(input.ToList()));
    }

    [Test]
    public void TestWhenInputEnumeratorFailsTest_LazyCollectionCanGetInitialItemsWithoutFailingTest()
    {
        var input = TestEnumerables.TestEnumerableThatFailsAfter(5);
        var lazy = Create(input);
        Assert.DoesNotThrow(() => lazy.Take(5).ToList());
        Assert.DoesNotThrow(() => lazy.Take(5).ToList());
    }

    [Test]
    public void TestWhenInputEnumeratorThrows_LazyCollectionThrowsOnlyWhenEnumerated()
    {
        var input = TestEnumerables.TestEnumerableThatThrows<int>();
        var lazy = Create(input);
        Assert.Throws<TestException>(() => lazy.ToList());
    }
    
    [Test]
    public void TestWhenInputEnumeratorThrowsOnMultipleEnumeration_LazyCollectionDoesNotThrow()
    {
        var input = TestEnumerables.TestEnumerableThatThrowsOnMultipleEnumeration();
        var lazy = Create(input);
        Assert.DoesNotThrow(() => lazy.ToList());
        Assert.DoesNotThrow(() => lazy.ToList());
        Assert.Throws<TestException>(() => input.ToList());
    }

    [Test]
    public void TestContainsKey()
    {
        var input = new Dictionary<int, int>();
        for (int i = 0; i < 10; i++)
        {
            input[i] = Random.Shared.Next();
        }

        var lazy = input.ToLazyDictionary();
        for (int i = 0; i < 10; i++)
        {
            Assert.That(lazy.ContainsKey(i), Is.True);
            Assert.That(lazy.ContainsKey(i), Is.True);
        }
        
        Assert.That(lazy.ContainsKey(-1), Is.False);
        Assert.That(lazy.ContainsKey(10), Is.False);
    }

    [Test]
    public void TestTryGetValue()
    {
        var input = new Dictionary<int, int>();
        for (int i = 0; i < 10; i++)
        {
            input[i] = Random.Shared.Next();
        }

        var lazy = input.ToLazyDictionary();
        for (int i = 0; i < 10; i++)
        {
            Assert.That(lazy.TryGetValue(i, out var val), Is.True);
            Assert.That(val, Is.EqualTo(input[i]));
            Assert.That(lazy.TryGetValue(i, out var val2), Is.True);
            Assert.That(val2, Is.EqualTo(input[i]));
        }
        
        Assert.That(lazy.TryGetValue(-1, out _), Is.False);
        Assert.That(lazy.TryGetValue(10, out _), Is.False);
    }

    [Test]
    public void TestIndexOperator()
    {
        var input = new Dictionary<int, int>();
        for (int i = 0; i < 10; i++)
        {
            input[i] = Random.Shared.Next();
        }

        var lazy = input.ToLazyDictionary();
        for (int i = 0; i < 10; i++)
        {
            Assert.That(lazy[i], Is.EqualTo(input[i]));
            Assert.That(lazy[i], Is.EqualTo(input[i]));
        }
        
        Assert.Throws<KeyNotFoundException>(() =>
        {
            _ = lazy[-1];
        });
        Assert.Throws<KeyNotFoundException>(() =>
        {
            _ = lazy[10];
        });
    }

    private static LazyDictionary<int, int> Create(IEnumerable<int> input)
    {
        return input.ToLazyDictionary(i => i, i => i);
    }
}