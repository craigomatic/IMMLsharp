using Imml.Runtime.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMLSharp.Desktop.Services
{
    public class CacheService : ICacheService
    {
        private string _CacheDir;
        private readonly Urho.Resources.ResourceCache _ResourceCache;

        public CacheService(string cacheDir, Urho.Resources.ResourceCache resourceCache)
        {
            _CacheDir = cacheDir;
            _ResourceCache = resourceCache;
            _ResourceCache.AddResourceDir(cacheDir, (uint)0);
        }

        public Task<byte[]> Retrieve(string key)
        {
            var filePath = _CacheDir + Path.DirectorySeparatorChar + key;

            if (!System.IO.File.Exists(filePath))
            {
                return Task.FromResult<byte[]>(null);
            }

            return Task.FromResult(File.ReadAllBytes(filePath));
        }

        public Task Store(string key, byte[] value)
        {
            var filePath = _CacheDir + Path.DirectorySeparatorChar + key;

            File.WriteAllBytes(filePath, value);

            return Task.FromResult(0);
        }
    }
}
