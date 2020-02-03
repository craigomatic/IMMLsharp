using Imml;
using System.Reflection;

namespace IMMLSharp.StereoKit
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