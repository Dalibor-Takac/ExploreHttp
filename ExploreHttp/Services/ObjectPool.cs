namespace ExploreHttp.Services;
public class ObjectPool<T> : IDisposable
    where T: class
{
    private enum PoolStatus
    {
        Available,
        Occupied
    }

    private readonly Dictionary<T, PoolStatus> _poolWithStatuses;
    private readonly int _capacity;
    private readonly Func<T> _factory;
    private readonly Action<T> _cleanup;

    public ObjectPool(int capacity, Func<T> factory, Action<T> cleanup)
    {
        _poolWithStatuses = new Dictionary<T, PoolStatus>(capacity);
        _capacity = capacity;
        _factory = factory;
        _cleanup = cleanup;
    }

    public T LeaseItem()
    {
        var freeItem = _poolWithStatuses.FirstOrDefault(x => x.Value == PoolStatus.Available);
        if (freeItem.Key is not null)
            return freeItem.Key;

        var newItem = _factory();
        if (_poolWithStatuses.Count < _capacity)
        {
            _poolWithStatuses.Add(newItem, PoolStatus.Occupied);
        }

        return newItem;
    }

    public void ReturnItem(T item)
    {
        if (_poolWithStatuses.TryGetValue(item, out var status) && status == PoolStatus.Occupied)
        {
            _cleanup(item);
            _poolWithStatuses[item] = PoolStatus.Available;
        }
        else
        {
            throw new InvalidOperationException("Attempting to return item to pool that is not managed by the pool. This is not allowed!");
        }
    }

    public void Dispose()
    {
        foreach (var item in _poolWithStatuses)
        {
            if (item is IDisposable disposableItem)
            {
                disposableItem.Dispose();
            }
        }
        _poolWithStatuses.Clear();
    }
}
