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

        private StaticModel _StaticModel;

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
            //put into scene with size scaled to unit of size 1 in the y-axis (uniform scale across x,y,z)
            var scalingFactor = 1 / _StaticModel.WorldBoundingBox.Size.Y;

            this.Node.SetWorldTransform(this.WorldPosition.ToUrhoVector3(), this.WorldRotation.ToQuaternion(), scalingFactor);

            var requestedScale = new Vector3(
                this.WorldSize.X * scalingFactor,
                this.WorldSize.Y * scalingFactor,
                this.WorldSize.Z * scalingFactor);

            this.Node.SetWorldScale(requestedScale);
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

            _StaticModel = this.Node.CreateComponent<StaticModel>();
            _StaticModel.Model = model;

            _StaticModel.CastShadows = this.CastShadows;

            this.LoadMaterials(_StaticModel, resourceCache);
            this.ApplyPhysics(this.Node);       

            return this.Node;
        }
    }
}
