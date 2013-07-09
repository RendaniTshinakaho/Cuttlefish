using System;
using System.Collections.Generic;
using Cuttlefish.Common;

namespace Cuttlefish.Caches.BasicInMemory
{
    /// <summary>
    ///     NOT MADE FOR PRODUCTION. FOR TESTING PURPOSES ONLY
    /// </summary>
    public class InMemoryDictionaryCache : ICache
    {
        private static readonly Dictionary<string, IAggregate> _Cache;

        static InMemoryDictionaryCache()
        {
            _Cache = new Dictionary<string, IAggregate>();
        }

        public void Cache<T>(T item) where T : IAggregate
        {
            string key = GetKey(item.AggregateIdentity, item.GetType());
            if (_Cache.ContainsKey(key))
            {
                _Cache[key] = item;
            }
            else
            {
                _Cache.Add(key, item);
            }
        }

        public T Fetch<T>(Guid id)
        {
            T returnValue = default(T);
            string key = GetKey(id, typeof (T));
            if (_Cache.ContainsKey(key))
            {
                returnValue = (T) _Cache[key];
            }
            return returnValue;
        }

        public object Fetch(Guid id, Type type)
        {
            object returnValue = null;
            string key = GetKey(id, type);
            if (_Cache.ContainsKey(key))
            {
                returnValue = _Cache[key];
            }
            return returnValue;
        }

        private string GetKey(Guid id, Type itemType)
        {
            return string.Format("{0}{1}", itemType.FullName, id);
        }
    }
}