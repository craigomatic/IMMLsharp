using Imml;
using Imml.Runtime;
using Imml.Runtime.Services;
using ImmlSharp.StereoKit.Services;
using IMMLSharp;
using StereoKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SK = StereoKit;

namespace ImmlSharp.StereoKit.Scene.Controls
{
    public class Model : Imml.Scene.Controls.Model, IRuntimeElement<SK.Model>
    {
        public SK.Model Node { get; private set; }


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
            
        }

        public void Dispose()
        {
        }

        public SK.Model Load(SK.Model parentNode)
        {
            var cacheService = DIContainer.Get<ICacheService>();

            var fileExtension = this.Source.FileExtension();
            var hash = this.Source.ToMD5() + fileExtension;
            var path = System.IO.Path.Combine((cacheService as CacheService).CacheDir.Path, hash);
            
            this.Node = SK.Model.FromFile(path, Shader.Find(DefaultIds.shaderPbr));
            
            this.LoadMaterials(this.Node, cacheService);

            return this.Node;
        }

        public void Render() 
        {
            if (!this.IsVisible)
            {
                return;
            }

            this.Node?.Draw(
                SK.Matrix.TRS(
                    this.Position.ToSKVec3(),
                    this.Rotation.ToSKQuat(),
                    this.Size.ToSKVec3()));
        }
    }
}
