using Imml.Runtime;
using StereoKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SK = StereoKit;

namespace ImmlSharp.StereoKit.Scene.Controls
{
    public class Text : Imml.Scene.Controls.Text, IRuntimeElement<Model>
    {
        public Model Node { get; private set; }

        public Model Load(Model parentNode)
        {            
            return this.Node;
        }

        public void ApplyLayout()
        {

        }

        public void Dispose()
        {
        }

        public Task AcquireResourcesAsync()
        {
            return Task.FromResult(0);
        }

        public void Render() 
        {
            var pointsToMetres = this.Size / 10;
            var alignment = _ResolveAlignment();

            SK.Text.Add(
                this.Value,
                SK.Matrix.TRS(
                    this.Position.ToSKVec3(),
                    this.Rotation.ToSKQuat(),
                    pointsToMetres),
                    alignment);
        }

        private SK.TextAlign _ResolveAlignment()
        {
            switch (this.Alignment)
            {
                case Imml.TextAlignment.Centre:
                case Imml.TextAlignment.Justify:
                    return TextAlign.Center;
                case Imml.TextAlignment.Left:
                    return TextAlign.XLeft;
                case Imml.TextAlignment.Right:
                    return TextAlign.XRight;
            }

            return TextAlign.Center;
        }
    }
}
