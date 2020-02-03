using Imml;
using Imml.Runtime;
using Imml.Runtime.Services;
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
    public class Primitive : Imml.Scene.Controls.Primitive, IRuntimeElement<SK.Model>
    {
        public SK.Model Node { get; private set; }

        public Primitive()
        {
            this.Size = Imml.Numerics.Vector3.One;
        }

        public async Task AcquireResourcesAsync()
        {
            var resourceAcquisitionService = DIContainer.Get<IResourceAcquisitionService>();

            var materialGroups = this.GetMaterialGroups();

            foreach (var item in materialGroups)
            {
                var texture = item.GetTexture();

                if (texture != null)
                {                    
                    try
                    {
                        await resourceAcquisitionService.AcquireResource(texture);
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to load Texture resource from {texture.Source}");
                    }
                }
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
            switch (this.Type)
            {
                case Imml.PrimitiveType.Box:
                    {
                        this.Node = SK.Model.FromMesh(Mesh.GenerateCube(Vec3.One), Default.Material.Copy());
                        break;
                    }
                case Imml.PrimitiveType.Cylinder:
                    {
                        this.Node = SK.Model.FromMesh(Mesh.GenerateCylinder(1,1,Vec3.Up), Default.Material.Copy());
                        break;
                    }
                case Imml.PrimitiveType.Cone:
                    {
                        //TODO: not supported by default set of primitives
                        break;
                    }
                case Imml.PrimitiveType.Plane:
                    {
                        this.Node = SK.Model.FromMesh(Mesh.GeneratePlane(Vec2.One), Default.Material.Copy());
                        break;
                    }
                case Imml.PrimitiveType.Sphere:
                    {
                        this.Node = SK.Model.FromMesh(Mesh.GenerateSphere(1), Default.Material.Copy());
                        break;
                    }
            }
            
            var resourceCache = DIContainer.Get<ICacheService>();

            this.LoadMaterials(this.Node, resourceCache);

            return this.Node;
        }

        public void Render() 
        {
            if (!this.IsVisible)
            {
                return;
            }

            var material = this.GetMaterialGroup(-1).GetMaterial();

            if (material != null)
            {
                this.Node?.Draw(
                    SK.Matrix.TRS(this.WorldPosition.ToSKVec3(),
                    Quat.FromAngles(
                        this.WorldRotation.X,
                        this.WorldRotation.Y,
                        this.WorldRotation.Z),
                    this.WorldSize.ToSKVec3()),
                    material.Diffuse.ToSKColor(material.Opacity));
            }
            else
            {
                this.Node?.Draw(
                    SK.Matrix.TRS(this.WorldPosition.ToSKVec3(),
                    Quat.FromAngles(
                        this.WorldRotation.X,
                        this.WorldRotation.Y,
                        this.WorldRotation.Z),
                    this.WorldSize.ToSKVec3()));
            }
        }
    }
}
