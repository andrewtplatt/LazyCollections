namespace LazyCollections.Tests;

[TestFixture]
public abstract class LazyCollectionTests<TLazy, TBase>
    where TLazy : LazyCollection<int, TBase>
    where TBase : ICollection<int>, new()
{
    [Test]
    public void LazyCollection_EnumeratesIdenticallyToInputEnumerable()
    {
        var input = new TBase();
        for (int i = 0; i < 10; i++)
        {
            input.Add(Random.Shared.Next());
        }

        var lazy = Create(input);

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

    protected abstract TLazy Create(IEnumerable<int> input);
}