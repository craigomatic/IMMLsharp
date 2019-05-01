using Imml.Runtime.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnityImml
{
    public class InMemoryCache : ICacheService
    {
        private Dictionary<string, byte[]> _Cache;

        public InMemoryCache()
        {
            _Cache = new Dictionary<string, byte[]>();
        }

        public Task<byte[]> Retrieve(string key)
        {
            if (_Cache.TryGetValue(key, out byte[] value))
            {
                return Task.FromResult(value);
            }

            return null;
        }

        public Task Store(string key, byte[] value)
        {
            _Cache.Add(key, value);

            return Task.FromResult(0);
        }
    }
}
