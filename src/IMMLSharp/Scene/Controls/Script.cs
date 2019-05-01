using Imml.Runtime;
using Imml.Runtime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;

namespace IMMLSharp.Scene.Controls
{
    public class Script : Imml.Scene.Controls.Script, IRuntimeElement<Node>
    {
        public Node Node { get; }

        public async Task AcquireResourcesAsync()
        {
            var resourceAcquisitionService = DIContainer.Get<IResourceAcquisitionService>();

            await resourceAcquisitionService.AcquireResource(this);
        }

        public void ApplyLayout()
        {

        }

        public void Dispose()
        {

        }

        public Node Load(Node parentNode)
        {
            //acquire the resource again (will come from cache)
            var resourceAcquisitionService = DIContainer.Get<IResourceAcquisitionService>();

            try
            {
                var bytes = Task.Run(() => resourceAcquisitionService.AcquireResource(this)).Result;
            }
            catch { }

            //TODO: load script

            return null;
        }
    }
}
