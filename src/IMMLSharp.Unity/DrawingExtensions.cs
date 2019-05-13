using System;
using System.Collections.Generic;
using System.Text;

namespace IMMLSharp.Unity
{
    public static class DrawingExtensions
    {
        public static UnityEngine.Color ToUnityColor(this Imml.Drawing.Color3 color3, float alpha)
        {
            return new UnityEngine.Color(
                color3.R,
                color3.G,
                color3.B,
                alpha);
        }
    }
}
