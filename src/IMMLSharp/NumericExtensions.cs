using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMLSharp
{
    public static class NumericExtensions
    {
        public static Urho.Vector3 ToUrhoVector3(this Imml.Numerics.Vector3 vector3)
        {
            return new Urho.Vector3(
                vector3.X,
                vector3.Y,
                vector3.Z);
        }
    }
}
