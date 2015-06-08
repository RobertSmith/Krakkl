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
            return GetOrAddExisting(key.ToString(), () => InitItem(key));
        }

        public static void RemoveItem(Guid key)
        {
            Cache.Remove(key.ToString());
        }

        public static void UpdateItem(Guid key, object item)
        {
            Cache.Set(key.ToString(), item);
        }

        private static T GetOrAddExisting<T>(string key, Func<T> valueFactory)
        {
            var newValue = new Lazy<T>(valueFactory);
            var oldValue = Cache.GetOrSet(key, newValue) as Lazy<T>;

            try
            {
                return (oldValue ?? newValue).Value;
            }
            catch
            {
                Cache.Remove(key);
                throw;
            }
        }

        private static object InitItem(Guid key)
        {
            var bookAggregate = new BookAggregate(key);

            if (bookAggregate.Key == Guid.Empty)
                throw new Exception("Book not found");

            return bookAggregate;
        }
    }
}