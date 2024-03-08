namespace Nebula;

internal struct CacheObject<T, U> : IDisposable where U : ICacheable, IDisposable
{
    private readonly Dictionary<T, U> s_cache = new Dictionary<T, U>();

    public CacheObject() { }

    public bool TryGetValue(T key, out U value)
    {
        return s_cache.TryGetValue(key, out value);
    }

    public bool TryGetKey(U value, out T key)
    {
        foreach (var item in s_cache)
        {
            if (EqualityComparer<U>.Default.Equals(item.Value, value))
            {
                key = item.Key;
                return true;
            }
        }
        key = default(T);
        return false;
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
