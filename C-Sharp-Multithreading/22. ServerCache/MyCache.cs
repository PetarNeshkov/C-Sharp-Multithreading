using System.Threading;

namespace MyCache
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class MyCache
    {
        private readonly Dictionary<string, object> cache;
        private readonly SemaphoreSlim locker;

        public MyCache()
        {
            this.cache = new Dictionary<string, object>();
            this.locker = new SemaphoreSlim(1);
        }

        public TItem GetOrCreate<TItem>(string key, Func<TItem> createItem)
            where TItem : class
        {
            locker.Wait();

            try
            {
                if (!cache.ContainsKey(key))
                {
                    cache[key] = createItem();
                }
            }
            finally
            {
                locker.Release();
            }

            return cache[key] as TItem;
        }

        public async Task<TItem> GetOrCreateAsync<TItem>(string key, Func<Task<TItem>> createItem)
            where TItem : class
        {
            await locker.WaitAsync();

            try
            {
                if (!cache.ContainsKey(key))
                {
                    cache[key] = await createItem();
                }
            }
            finally
            {
                locker.Release();
            }
            
            return  cache[key] as TItem;
        }
    }
}