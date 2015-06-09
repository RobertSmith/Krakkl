using System;
using Krakkl.Authorship.Core;
using Microsoft.Framework.Caching.Memory;

namespace Krakkl.Authorship.Cache
{
    internal static class BookAggregateCache
    {
        private static readonly MemoryCacheOptions Options = new MemoryCacheOptions();
        private static readonly MemoryCache Cache = new MemoryCache(Options);

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
            Cache.Set(key.ToString(), item);
        }
    }
}