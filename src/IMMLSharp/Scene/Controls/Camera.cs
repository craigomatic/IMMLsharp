using Imml.Runtime;
using Imml.Scene.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;

namespace IMMLSharp.Scene.Controls
{
    public class Camera : Imml.Scene.Controls.Camera, IRuntimeElement<Node>
    {
        public Node Node { get; private set; }

        public void ApplyLayout()
        {
            if (this.Container is ImmlDocument &&
                (this.Container as ImmlDocument).Camera == this.Name)
            {
                this.Node.SetWorldPosition(this.WorldPosition.ToUrhoVector3());

                System.Diagnostics.Debug.WriteLine($"Active camera: {this.Name}, position: {this.WorldPosition}");
            }
        }

        public void Dispose()
        {
            this.Node.Remove();
            this.Node.Dispose();
        }

        public Node Load(Node parentNode)
        {
            this.Node = parentNode.Scene.CreateChild(this.Name);

            var cameraComponent = this.Node.CreateComponent<Urho.Camera>();
            cameraComponent.NearClip = this.NearPlane;
            cameraComponent.FarClip = this.FarPlane;
            cameraComponent.Fov = this.FOV;
            cameraComponent.Orthographic = this.Projection == Imml.ProjectionType.Isometric;

            return this.Node;
        }

        public Task AcquireResourcesAsync()
        {
            return Task.FromResult(0);
        }
    }
}
