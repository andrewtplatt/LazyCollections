namespace LazyCollections.Tests;

public static class TestEnumerables
{
    public static IEnumerable<T> TestEnumerableThatThrows<T>()
    {
        return Impl().Skip(1);

        IEnumerable<T> Impl()
        {
            yield return default!;
            throw new TestException();
        }
    }

    public static IEnumerable<int> TestEnumerableThatFailsAfter(uint i)
    {
        for (int j = 0; j <= i; j++)
        {
            if (j == i)
            {
                Assert.Fail(i.ToString());
            }

            yield return j;
        }
    }

    public static IEnumerable<int> TestEnumerableThatThrowsOnMultipleEnumeration()
    {
        bool enumerated = false;
        return Impl();

        IEnumerable<int> Impl()
        {
            if (enumerated)
            {
                throw new TestException();
            }

            enumerated = true;
            yield return 1;
            yield return 2;
        }
    }
}