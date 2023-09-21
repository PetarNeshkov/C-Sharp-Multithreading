using System.Threading;

namespace MyCache
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    public class MyConcurrentCache
    {
        private readonly ConcurrentDictionary<string, object> cache;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> lockers;

        public MyConcurrentCache()
        {
            this.cache = new ConcurrentDictionary<string, object>();
            this.lockers = new ConcurrentDictionary<string, SemaphoreSlim>();
        }

        public TItem GetOrCreate<TItem>(string key, Func<TItem> createItem)
            where TItem : class
        {
            if (!cache.TryGetValue(key, out var value ))
            {
                var locker = lockers.GetOrAdd(key, _ => new SemaphoreSlim(1));

                locker.Wait();

                try
                {
                    if (!cache.ContainsKey(key))
                    {
                        value = cache.GetOrAdd(key, _ => createItem());
                    }
                }
                finally
                {
                    locker.Release();
                }
            }

            return value as TItem;
        }

        public async Task<TItem> GetOrCreateAsync<TItem>(string key, Func<Task<TItem>> createItem)
            where TItem : class
        {
            if (!cache.TryGetValue(key, out var value ))
            {
                var locker = lockers.GetOrAdd(key, _ => new SemaphoreSlim(1));

               await locker.WaitAsync();

                try
                {
                    if (!cache.ContainsKey(key))
                    {
                        value = cache.GetOrAdd(key, _ => createItem());
                    }
                }
                finally
                {
                    locker.Release();
                }
            }

            return value as TItem;
        }
    }
}
