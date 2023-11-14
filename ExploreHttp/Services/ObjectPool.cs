namespace ExploreHttp.Services;
public class ObjectPool<T> : IDisposable
    where T: class
{
    private enum PoolStatus
    {
        Available,
        Occupied
    }

    public class PooledObject : IDisposable
    {
        private readonly ObjectPool<T> _originatingPool;
        public T Object { get; }

        public PooledObject(ObjectPool<T> originatingPool, T obj)
        {
            Object = obj;
            _originatingPool = originatingPool;
        }

        public void Dispose()
        {
            if (_originatingPool is not null)
            {
                _originatingPool.ReturnItem(Object);
            }
            else
            {
                if (Object is IDisposable d)
                    d.Dispose();
            }
        }
    }

    private readonly Dictionary<T, PoolStatus> _poolWithStatuses;
    private readonly int _capacity;
    private readonly Func<T> _factory;
    private readonly Action<T> _cleanup;

    private readonly object _lockObj;

    public ObjectPool(int capacity, Func<T> factory, Action<T> cleanup)
    {
        _poolWithStatuses = new Dictionary<T, PoolStatus>(capacity);
        _capacity = capacity;
        _factory = factory;
        _cleanup = cleanup;
        _lockObj = new object();
    }

    public PooledObject LeaseItem()
    {
        lock(_lockObj)
        {
            var freeItem = _poolWithStatuses.FirstOrDefault(x => x.Value == PoolStatus.Available);
            if (freeItem.Key is not null)
            {
                _poolWithStatuses[freeItem.Key] = PoolStatus.Occupied;
                return new PooledObject(this, freeItem.Key);
            }

            var newItem = _factory();
            if (_poolWithStatuses.Count < _capacity)
            {
                _poolWithStatuses.Add(newItem, PoolStatus.Occupied);
            }

            return new PooledObject(null, newItem);
        }
    }

    public void ReturnItem(T item)
    {
        lock (_lockObj)
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
