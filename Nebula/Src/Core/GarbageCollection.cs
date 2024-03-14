namespace Nebula;

internal static class GargabeCollection
{
    private static uint s_collectionCount = 0;
    private static float s_timer = 0f;
    private static float s_collectionIntervall;

    private const uint c_maxCollectionQueueCount = 2;

    internal static void Init(float collectionIntervall)
    {
        Logger.EngineInfo($"Initialising gargabe collection with a collection intervall of {collectionIntervall} seconds");
        s_collectionIntervall = collectionIntervall;
        s_timer = 0f;
    }

    public static void QueueCollection()
    {
        s_collectionCount = Math.Min(s_collectionCount + 1, c_maxCollectionQueueCount);
        Logger.EngineVerbose($"Queueing garbage collection, {s_collectionCount} collections queued");
    }

    internal static void Update(float deltaTime)
    {
        s_timer += deltaTime;
        if (s_timer >= s_collectionIntervall)
        {
            if (s_collectionCount > 0)
            {
                GC.Collect();
                s_collectionCount = Math.Max(s_collectionCount - 1, 0);
                Logger.EngineDebug($"Collecting gargabe, {s_collectionCount} collections queued");
            }
            s_timer = 0f;
        }
    }
}
