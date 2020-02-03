using StereoKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SK = StereoKit;

namespace ImmlSharp.StereoKit
{
    public static class NumericExtensions
    {
        public static SK.Vec3 ToSKVec3(this Imml.Numerics.Vector3 vector3)
        {
            //StereoKit is using a coordinate system where:
            // X increases to the right
            // Y increases up
            // Z increases towards the viewer
            return new SK.Vec3(
                vector3.X,
                vector3.Y,
                vector3.Z);
        }

        public static SK.Quat ToSKQuat(this Imml.Numerics.Vector3 vector3)
        {
            //StereoKit is using a coordinate system where:
            // X increases to the right
            // Y increases up
            // Z increases towards the viewer
            return Quat.FromAngles(
                vector3.X,
                vector3.Y,
                vector3.Z);
        }
    }
}
