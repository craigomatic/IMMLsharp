using Imml;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace UnityImml
{
    public class UnityElementFactory : ElementFactory
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
