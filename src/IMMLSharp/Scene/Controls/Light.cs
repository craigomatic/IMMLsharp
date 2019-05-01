using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imml;
using Imml.Runtime;
using Urho;

namespace IMMLSharp.Scene.Controls
{
    public class Light : Imml.Scene.Controls.Light, IRuntimeElement<Node>
    {
        public Node Node { get; private set; }

        public Node Load(Node parentNode)
        {
            this.Node = parentNode.CreateChild(this.Name);

            this.LoadPositionalElement(this.Node);

            var lightComponent = this.Node.CreateComponent<Urho.Light>();
            lightComponent.LightType = _ResolveLightType(this.Type);
            lightComponent.Color = this.Diffuse.ToColor();
            lightComponent.CastShadows = this.CastShadows;
            lightComponent.Enabled = this.Enabled;

            if (this.Type == Imml.LightType.Directional)
            {
                this.Node.SetWorldDirection(this.WorldRotation.ToUrhoVector3());
            }
            else
            {
                
                lightComponent.Range = this.Range;
            }

            return this.Node;
        }

        private static Urho.LightType _ResolveLightType(Imml.LightType type)
        {
            switch (type)
            {
                case Imml.LightType.Directional:
                    return Urho.LightType.Directional;
                case Imml.LightType.Point:
                    return Urho.LightType.Point;
                case Imml.LightType.Spot:
                    return Urho.LightType.Spot;
            }

            throw new NotSupportedException();
        }

        public void ApplyLayout()
        {
        }

        public void Dispose()
        {
            this.Node.Remove();
            this.Node.Dispose();
        }

        public Task AcquireResourcesAsync()
        {
            return Task.FromResult(0);
        }
    }
}
