using Imml;
using Imml.Runtime;
using Imml.Runtime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;
using Urho.Shapes;

namespace IMMLSharp.Scene.Controls
{
    public class Primitive : Imml.Scene.Controls.Primitive, IRuntimeElement<Node>
    {
        public Node Node { get; private set; }

        public Primitive()
        {
            this.Size = Imml.Numerics.Vector3.One;
        }

        public async Task AcquireResourcesAsync()
        {
            var resourceAcquisitionService = DIContainer.Get<IResourceAcquisitionService>();

            //primitives may have texture elements as children that need to be acquired
            if (this.HasChildren)
            {
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
        }

        public void ApplyLayout()
        {
            this.Node.SetTransform(
                this.WorldPosition.ToUrhoVector3(),
                this.WorldRotation.ToQuaternion(),
                this.WorldSize.ToUrhoVector3());
        }

        public void Dispose()
        {
            this.Node.Remove();
            this.Node.Dispose();
        }

        public Node Load(Node parentNode)
        {
            this.Node = parentNode.CreateChild(this.Name);
                        
            Shape shape = null;

            switch (this.Type)
            {
                case Imml.PrimitiveType.Box:
                    {
                        shape = this.Node.CreateComponent<Box>();                
                        break;
                    }
                case Imml.PrimitiveType.Cone:
                    {
                        shape = this.Node.CreateComponent<Cone>();
                        break;
                    }
                case Imml.PrimitiveType.Cylinder:
                    {
                        shape = this.Node.CreateComponent<Cylinder>();
                        break;
                    }
                case Imml.PrimitiveType.Plane:
                    {
                        shape = this.Node.CreateComponent<Urho.Shapes.Plane>();
                        break;
                    }
                case Imml.PrimitiveType.Sphere:
                    {
                        shape = this.Node.CreateComponent<Sphere>();
                        break;
                    }
            }            

            var resourceCache = DIContainer.Get<Urho.Resources.ResourceCache>();

            this.LoadMaterials(shape, resourceCache);
            this.ApplyPhysics(this.Node);

            return this.Node;
        }
    }
}
