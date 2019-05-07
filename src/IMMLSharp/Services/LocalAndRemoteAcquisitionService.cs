using Imml.ComponentModel;
using Imml.Runtime;
using Imml.Runtime.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMLSharp.Services
{
    public class LocalAndRemoteAcquisitionService : IResourceAcquisitionService
    {
        public ICacheService CacheService { get; private set; }

        /// <summary>
        /// The full path to the document the resources are being requested for
        /// </summary>
        public string DocumentRootUri { get; set; }

        private ResourceAcquisitionService _HttpAcquisitionService;
       
        public LocalAndRemoteAcquisitionService(ICacheService cacheService)
        {
            this.CacheService = cacheService;

            _HttpAcquisitionService = new ResourceAcquisitionService(cacheService);
        }

        public async Task<byte[]> AcquireResource(ISourcedElement sourcedElement)
        {
            //naive implementation, unless http or https assume file on disk
            if (string.IsNullOrWhiteSpace(this.DocumentRootUri) ||
                this.DocumentRootUri.StartsWith("http"))
            {
                return await _HttpAcquisitionService.AcquireResource(sourcedElement);
            }

            var fInfo = new FileInfo(this.DocumentRootUri);
            var fullPathToSourcedElement = Path.Combine(fInfo.DirectoryName, sourcedElement.Source);

            //prefix sourceUri with document root URI to avoid incorrect cache hits for files with same name/relative path but different directories
            sourcedElement.Source = fullPathToSourcedElement;

            //try cache first
            var fileExtension = sourcedElement.Source.FileExtension();
            var hash = sourcedElement.Source.ToMD5() + fileExtension;
            var bytes = await this.CacheService.Retrieve(hash);

            if (bytes != null)
            {
                System.Diagnostics.Debug.WriteLine($"Cache hit for {sourcedElement.Source}");
                return bytes;
            }

            System.Diagnostics.Debug.WriteLine($"Cache miss on {sourcedElement.Source}");

            bytes = System.IO.File.ReadAllBytes(fullPathToSourcedElement);

            await this.CacheService.Store(hash, bytes);

            return bytes;
        }
    }
}
