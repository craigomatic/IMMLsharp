using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace IMMLSharp
{
    public static class FileExtensions
    {
        public async static Task<Urho.MemoryBuffer> ToUrhoBuffer(this IStorageFile storageFile)
        {            
            var stream = await storageFile.OpenStreamForReadAsync();
            var bytes = new byte[(int)stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);

            return new Urho.MemoryBuffer(bytes);
        }        
    }
}
