using Imml;
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
    public class Model : Imml.Scene.Controls.Model, IRuntimeElement<Node>
    {
        public Node Node { get; private set; }

        public Model()
        {
            this.Size = Imml.Numerics.Vector3.One;
        }

        public async Task AcquireResourcesAsync()
        {
            var resourceAcquisitionService = DIContainer.Get<IResourceAcquisitionService>();

            try
            {
                //just acquire the resource, which will drop it into the resource cache so it can be used later during load
                await resourceAcquisitionService.AcquireResource(this);

                if (this.HasChildren)
                {
                    var materialGroups = this.GetMaterialGroups();

                    foreach (var item in materialGroups)
                    {
                        var texture = item.GetTexture();

                        if (texture != null)
                        {
                            await resourceAcquisitionService.AcquireResource(texture);
                        }
                    }
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load {this.GetType().Name} resource from {this.Source}");
            }
        }

        public void ApplyLayout()
        {
            this.Node.SetWorldPosition(this.WorldPosition.ToUrhoVector3());
            this.Node.SetWorldRotation(this.WorldRotation.ToQuaternion());

            this.Node.SetScale(100);
        }

        public void Dispose()
        {
            this.Node.Remove();
            this.Node.Dispose();
        }

        public Node Load(Node parentNode)
        {
            this.Node = parentNode.CreateChild(this.Name);

            this.LoadCubicElement(this.Node);

            var resourceCache = DIContainer.Get<Urho.Resources.ResourceCache>();

            var fileExtension = this.Source.FileExtension();
            var hash = this.Source.ToMD5() + fileExtension;
            var model = resourceCache.GetModel(hash);

            var staticModel = this.Node.CreateComponent<StaticModel>();
            staticModel.Model = model;

            staticModel.CastShadows = this.CastShadows;

            this.LoadMaterials(staticModel, resourceCache);
            this.ApplyPhysics(this.Node);       

            return this.Node;
        }
    }
}
