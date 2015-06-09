using System;
using System.Runtime.Caching;

namespace Krakkl.Authorship.Cache
{
    internal static class BookAggregateCache
    {
        private static readonly MemoryCache Cache = new MemoryCache("BookAggregate");

        public static object Get(Guid key)
        {
            return Cache.Get(key.ToString());
        }

        public static void RemoveItem(Guid key)
        {
            Cache.Remove(key.ToString());
        }

        public static void UpdateItem(Guid key, object item)
        {
            var policy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromSeconds(30)
            };

            Cache.Set(key.ToString(), item, policy);
        }
    }
}