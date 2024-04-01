namespace LazyCollections.Tests;

[TestFixture]
[TestOf(typeof(LazyHashSet<>))]
public class LazyHashSetTests : LazyCollectionTests<LazyHashSet<int>, HashSet<int>>
{
    protected override LazyHashSet<int> Create(IEnumerable<int> input)
    {
        return input.ToLazyHashSet();
    }
}