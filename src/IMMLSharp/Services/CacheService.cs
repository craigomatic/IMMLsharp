using Imml.Runtime.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace IMMLSharp.Services
{
    public class CacheService : ICacheService
    {
        public StorageFolder CacheDir { get; set; }

        private readonly Urho.Resources.ResourceCache _ResourceCache;

        public CacheService(StorageFolder cacheDir, Urho.Resources.ResourceCache resourceCache)
        {
            this.CacheDir = cacheDir;

            _ResourceCache = resourceCache;
            _ResourceCache.AddResourceDir(cacheDir.Path, (uint)0);
        }

        public async Task Store(string key, byte[] value)
        {
            var cacheFile = await this.CacheDir.CreateFileAsync(key, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(cacheFile, value);
        }

        public async Task<byte[]> Retrieve(string key)
        {
            var item = await this.CacheDir.TryGetItemAsync(key);

            if (item != null)
            {
                var stream = await ((IStorageFile)item).OpenStreamForReadAsync();
                var buffer = new byte[stream.Length];

                await stream.ReadAsync(buffer, 0, buffer.Length);

                return buffer;
            }

            return null;
        }
    }
}
