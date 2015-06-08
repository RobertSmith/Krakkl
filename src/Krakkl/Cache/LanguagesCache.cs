using System;
using Microsoft.Framework.Caching.Memory;

namespace Krakkl.Cache
{
    public static class LanguagesCache
    {
        private static readonly MemoryCacheOptions Options = new MemoryCacheOptions();
        private static readonly MemoryCache Cache = new MemoryCache(Options);

        public static object GetAll()
        {
            return GetOrAddExisting("All", InitItem);
        }

        public static void RemoveItem(string key)
        {
            Cache.Remove(key);
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
            var lang = new Query.Language();
            return lang.Languages;
        }
    }
}