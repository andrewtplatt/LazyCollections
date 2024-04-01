namespace LazyCollections.Tests;

[TestFixture]
[TestOf(typeof(LazyList<>))]
public class LazyListTests : LazyCollectionTests<LazyList<int>, List<int>>
{
    protected override LazyList<int> Create(IEnumerable<int> input)
    {
        return input.ToLazyList();
    }
}