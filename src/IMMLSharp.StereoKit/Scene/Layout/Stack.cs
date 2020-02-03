using Imml.ComponentModel;
using Imml.Runtime;
using StereoKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmlSharp.StereoKit.Scene.Layout
{
    public class Stack : Imml.Scene.Layout.Stack, IRuntimeElement<Model>
    {
        public Model Node { get; set; }

        public Task AcquireResourcesAsync()
        {
            return Task.FromResult(0);
        }

        public void ApplyLayout()
        {
            //for each child element, need to calculate their world position based on stack position and spacing
            var cubicElements = this.Elements.OfType<CubicElement>();

            var currentItem = cubicElements.First();
            currentItem.WorldPosition = this.WorldPosition;

            foreach (var item in cubicElements)
            {
                if (item == currentItem)
                {
                    continue;
                }

                var nextPosition = currentItem.WorldPosition + this.Spacing;
                item.WorldPosition = nextPosition;

                currentItem = item;
            }

            var runtimeElements = cubicElements.OfType<IRuntimeElement<Model>>();

            foreach (var item in runtimeElements)
            {
                item.ApplyLayout();
            }
        }

        public void Dispose()
        {
            
        }

        public Model Load(Model parentNode)
        {
            //stack is a layout element, just create a node in the scene for child elements to attach to
            //this.Node = parentNode..CreateChild(this.Name);

            return this.Node;
        }

        public void Render() { }
    }
}
