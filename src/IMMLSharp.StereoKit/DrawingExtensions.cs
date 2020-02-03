using Imml;
using Imml.ComponentModel;
using Imml.Runtime.Services;
using StereoKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using SK = StereoKit;

namespace ImmlSharp.StereoKit
{
    public static class DrawingExtensions
    {
        public static SK.Color ToSKColor(this Imml.Drawing.Color3 color3, float opacity = 1)
        {
            return new SK.Color(color3.R, color3.G, color3.B, opacity);
        }
    }
}
