using Imml;
using Imml.Scene.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IMMLSharp
{
    public class ImmlElementFactory : ElementFactory
    {
        public override Assembly ResolveAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }

        public override ImmlElement Create(string elementName, IImmlElement parentElement)
        {
            var element = base.Create(elementName, parentElement);

            if (element == null)
            {
                System.Diagnostics.Debug.WriteLine($"Missing implementation for {elementName}");
                return ElementFactory.Default.Create(elementName, parentElement);
            }            

            return element;
        }
    }
}
