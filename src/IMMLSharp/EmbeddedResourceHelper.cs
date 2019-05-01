using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace IMMLSharp
{
    public static class EmbeddedResourceHelper
    {
        public async static Task<MemoryStream> GetMemoryStream(string fileUri)
        {
            StorageFile a = await StorageFile.GetFileFromApplicationUriAsync(new Uri(fileUri));
            
            var file = await a.OpenReadAsync();
            var stream = file.AsStreamForRead();

            if (stream == null)
                return null;

            try
            {
                MemoryStream ms = new MemoryStream();
                byte[] buffer = new byte[1024];
                int bytesRead = 0;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    ms.Write(buffer, 0, bytesRead);
                }

                ms.Position = 0;

                return ms;
            }
            catch
            {
                return null;
            }
        }
    }
}
