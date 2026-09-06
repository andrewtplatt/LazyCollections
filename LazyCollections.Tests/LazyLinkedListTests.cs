namespace LazyCollections.Tests;

[TestFixture]
public class LazyLinkedListTests
{
    [Test]
    public void LazyLinkedList_EnumeratesIdenticallyToInputEnumerable()
    {
        ICollection<int> input = new LinkedList<int>();
        for (int i = 0; i < 10; i++)
        {
            input.Add(Random.Shared.Next());
        }

        var lazy = input.ToLazyLinkedList();

        using var groundTruth = input.GetEnumerator();
        using var lazily = lazy.GetEnumerator();
        while (groundTruth.MoveNext())
        {
            Assert.Multiple(() =>
            {
                Assert.That(lazily.MoveNext());
                Assert.That(groundTruth.Current, Is.EqualTo(lazily.Current));
            });
        }

        Assert.Multiple(() =>
        {
            Assert.That(!lazily.MoveNext());
            Assert.That(lazy.ToList(), Is.EqualTo(input.ToList()));
        });
    }

    [Test]
    public void LazyLinkedList_CanBeWalkedManually()
    {
        var input = new LinkedList<int>();
        for (int i = 0; i < 10; i++)
        {
            input.AddLast(Random.Shared.Next());
        }

        var lazy = input.ToLazyLinkedList();
        var groundTruth = input.First;
        var test = lazy.First;
        Assert.That(test?.Value, Is.EqualTo(groundTruth?.Value));

        while (groundTruth?.Next is not null)
        {
            groundTruth = groundTruth.Next;
            test = test.Next;
            Assert.That(test?.Value, Is.EqualTo(groundTruth?.Value));
        }

        Assert.That(test.Next, Is.Null);

        while (groundTruth?.Previous is not null)
        {
            groundTruth = groundTruth.Previous;
            test = test.Previous;
            Assert.That(test?.Value, Is.EqualTo(groundTruth?.Value));
        }

        Assert.That(test.Previous, Is.Null);
    }

    [Test]
    public void LazyLinkedList_FirstAndLastAreCorrect()
    {
        var input = new LinkedList<int>();
        for (int i = 0; i < 10; i++)
        {
            input.AddLast(Random.Shared.Next());
        }

        var lazy = input.ToLazyLinkedList();
        Assert.Multiple(() =>
        {
            Assert.That(lazy.First!.Value, Is.EqualTo(input.First!.Value));
            Assert.That(lazy.Last!.Value, Is.EqualTo(input.Last!.Value));
        });
    }

    [Test]
    public void TestWhenInputEnumeratorFailsTest_LazyLinkedListCanGetInitialItemsWithoutFailingTest()
    {
        var input = TestEnumerables.TestEnumerableThatFailsAfter(5);
        var lazy = input.ToLazyLinkedList();
        Assert.DoesNotThrow(() => lazy.Take(5).ToList());
        Assert.DoesNotThrow(() => lazy.Take(5).ToList());
    }

    [Test]
    public void TestWhenInputEnumeratorThrows_LazyLinkedListThrowsOnlyWhenEnumerated()
    {
        var input = TestEnumerables.TestEnumerableThatThrows<int>();
        var lazy = input.ToLazyLinkedList();
        Assert.Throws<TestException>(() => lazy.ToList());
    }

    [Test]
    public void TestWhenInputEnumeratorThrowsOnMultipleEnumeration_LazyLinkedListDoesNotThrow()
    {
        var input = TestEnumerables.TestEnumerableThatThrowsOnMultipleEnumeration();
        var lazy = input.ToLazyLinkedList();
        Assert.DoesNotThrow(() => lazy.ToList());
        Assert.DoesNotThrow(() => lazy.ToList());
        Assert.Throws<TestException>(() => input.ToList());
    }
}
