using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMLSharp
{
    public static class NumericExtensions
    {
        public static UnityEngine.Vector3 ToUnityVector3(this Imml.Numerics.Vector3 vector3)
        {
            return new UnityEngine.Vector3(
                vector3.X,
                vector3.Y,
                vector3.Z);
        }       
    }
}
