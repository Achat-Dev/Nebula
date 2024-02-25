namespace Nebula;

internal struct CacheObject<T, U> : IDisposable where U : class, ICacheable, IDisposable
{
    private readonly Dictionary<T, U> s_cache = new Dictionary<T, U>();

    public CacheObject() { }

    public bool GetValue(T key, out U value)
    {
        return s_cache.TryGetValue(key, out value);
    }

    public T GetKey(U value)
    {
        foreach (var item in s_cache)
        {
            if (item.Value == value)
            {
                return item.Key;
            }
        }
        return default(T);
    }

    public void CacheData(T key, U value)
    {
        s_cache.Add(key, value);
    }

    public void RemoveData(T key)
    {
        s_cache.Remove(key);
    }

    public void Dispose()
    {
        Logger.EngineDebug($"Disposing cache object of type {typeof(U)}");
        foreach (var item in s_cache)
        {
            item.Value.Dispose();
        }
        s_cache.Clear();
    }
}
