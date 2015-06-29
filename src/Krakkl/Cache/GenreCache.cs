using System;
using Microsoft.Framework.Caching.Memory;

namespace Krakkl.Cache
{
    public static class GenreCache
    {
        private static readonly MemoryCacheOptions Options = new MemoryCacheOptions();
        private static readonly MemoryCache Cache = new MemoryCache(Options);

        public static object GetAll()
        {
            return GetOrAddExisting("All", InitItem);
        }

        public static void RemoveItem()
        {
            Cache.Remove("All");
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

        private static object InitItem()
        {
            var genre = new Query.GenreQueries();
            return genre.Genres;
        }
    }
}