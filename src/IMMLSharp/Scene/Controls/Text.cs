using Imml;
using Imml.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;
using Urho.Gui;

namespace IMMLSharp.Scene.Controls
{
    public class Text : Imml.Scene.Controls.Text, IRuntimeElement<Node>
    {
        public Node Node { get; private set; }

        public Node Load(Node parentNode)
        {
            this.Node = parentNode.CreateChild(this.Name);

            this.LoadPositionalElement(this.Node);

            var textComponent = this.Node.CreateComponent<Text3D>();
            textComponent.Text = this.Value;
            textComponent.SetColor(this.Colour.ToColor());
            textComponent.TextAlignment = _ResolveAlignment(this.Alignment);

            //TODO: honour the requested font 
            textComponent.SetFont(CoreAssets.Fonts.AnonymousPro, this.Size);

            return this.Node;
        }

        private static Urho.Gui.HorizontalAlignment _ResolveAlignment(TextAlignment alignment)
        {
            switch (alignment)
            {
                case TextAlignment.Centre:
                    {
                        return Urho.Gui.HorizontalAlignment.Center;
                    }
                case TextAlignment.Justify:
                    {
                        return Urho.Gui.HorizontalAlignment.Custom; //TODO: this may not be a good match
                    }
                case TextAlignment.Left:
                    {
                        return Urho.Gui.HorizontalAlignment.Left;
                    }
                case TextAlignment.Right:
                    {
                        return Urho.Gui.HorizontalAlignment.Right;
                    }
            }

            return Urho.Gui.HorizontalAlignment.Center;
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
