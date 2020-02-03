using Imml.Runtime.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ImmlSharp.StereoKit.Services
{
    public class CacheService : ICacheService
    {
        public StorageFolder CacheDir { get; set; }

        public CacheService(StorageFolder cacheDir)
        {
            this.CacheDir = cacheDir;
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