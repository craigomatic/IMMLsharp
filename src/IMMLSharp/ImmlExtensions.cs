using Imml;
using Imml.ComponentModel;
using Imml.Numerics;
using Imml.Runtime;
using Imml.Scene.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;
using Urho.Audio;
using Urho.Gui;
using Urho.Physics;
using Urho.Shapes;

namespace IMMLSharp
{
    public static class ImmlExtensions
    {
        public static void ApplyPhysics(this IPhysicsHostElement physicsHostElement, Node node)
        {            
            var physicsElement = physicsHostElement.GetPhysics();

            if (physicsElement == null)
            {
                return;
            }

            var rigidBody = node.CreateComponent<RigidBody>();
            rigidBody.Mass = physicsElement.Movable ? physicsElement.Weight : 0;
            rigidBody.Enabled = physicsElement.Enabled;

            var collisionShape = node.CreateComponent<CollisionShape>();
            collisionShape.Enabled = physicsElement.Enabled;

            switch (physicsElement.Bounding)
            {
                case BoundingType.ConvexHull:
                    {
                        collisionShape.SetConvexHull(node.GetComponent<Urho.StaticModel>().Model, 1, Urho.Vector3.One, Urho.Vector3.Zero, Urho.Quaternion.Identity);
                        break;
                    }
                case BoundingType.Cylinder:
                    {
                        collisionShape.SetCylinder(1, 1, Urho.Vector3.Zero, Urho.Quaternion.Identity);
                        break;
                    }
                case BoundingType.Sphere:
                    {
                        collisionShape.SetSphere(1, Urho.Vector3.Zero, Urho.Quaternion.Identity);
                        break;
                    }
                default:
                    {
                        collisionShape.SetBox(Urho.Vector3.One, Urho.Vector3.Zero, Urho.Quaternion.Identity);
                        break;
                    }
            }            
        }

        public static void LoadCubicElement(this ICubicElement cubicElement, Node node)
        {
            cubicElement.LoadPositionalElement(node);

            node.SetWorldScale(cubicElement.WorldSize.ToUrhoVector3());
        }

        public static void LoadPositionalElement(this IPositionalElement positionalElement, Node node)
        {
            node.SetWorldPosition(positionalElement.WorldPosition.ToUrhoVector3());

            var quaternion = positionalElement.WorldRotation.ToQuaternion();
            node.SetWorldRotation(quaternion);
        }

        public static void LoadMaterials(this IMaterialHostElement materialHostElement, StaticModel staticModel, Urho.Resources.ResourceCache resourceCache)
        {
            var materialGroups = materialHostElement.GetMaterialGroups();

            foreach (var materialGroup in materialGroups)
            {
                var material = materialGroup.GetMaterial();
                var texture = materialGroup.GetTexture();
                var videoTexture = materialGroup.GetVideo();
                var webTexture = materialGroup.GetWeb();

                if (material != null)
                {
                    if (materialGroup.Id == -1)
                    {
                        staticModel.SetMaterial(Material.FromColor(material.Diffuse.ToColor()));
                    }
                    else
                    {
                        staticModel.SetMaterial((uint)materialGroup.Id, Material.FromColor(material.Diffuse.ToColor()));
                    }
                }

                if (texture != null)
                {
                    var fileExtension = texture.Source.FileExtension();
                    var hash = texture.Source.ToMD5() + fileExtension;

                    var img = resourceCache.GetImage(hash);

                    if (materialGroup.Id == -1)
                    {
                        staticModel.SetMaterial(Material.FromImage(img));
                    }
                    else
                    {
                        staticModel.SetMaterial((uint)materialGroup.Id, Material.FromImage(img));
                    }
                }
            }
        }

        public static Urho.Quaternion ToQuaternion(this Imml.Numerics.Vector3 worldRotation)
        {
            var xRad = Angle.FromDegrees(worldRotation.X).Radians;
            var yRad = Angle.FromDegrees(worldRotation.Y).Radians;
            var zRad = Angle.FromDegrees(worldRotation.Z).Radians;

            var c1 = Math.Cos(xRad / 2f);
            var s1 = Math.Sin(xRad / 2f);
            var c2 = Math.Cos(yRad / 2f);
            var s2 = Math.Sin(yRad / 2f);
            var c3 = Math.Cos(zRad / 2f);
            var s3 = Math.Sin(zRad / 2f);
            var c1c2 = c1 * c2;
            var s1s2 = s1 * s2;
            var w = c1c2 * c3 - s1s2 * s3;
            var x = c1c2 * s3 + s1s2 * c3;
            var y = s1 * c2 * c3 + c1 * s2 * s3;
            var z = c1 * s2 * c3 - s1 * c2 * s3;

            return new Urho.Quaternion(
                Convert.ToSingle(x), 
                Convert.ToSingle(y), 
                Convert.ToSingle(z), 
                Convert.ToSingle(w));
        }

        public static Urho.Color ToColor(this Imml.Drawing.Color3 color)
        {
            return new Color(color.R, color.G, color.B); 
        }

        public static Urho.Physics.ShapeType ToUrhoShapeType(this BoundingType boundingType)
        {
            switch (boundingType)
            {
                case BoundingType.Box:
                    return ShapeType.Box;
                case BoundingType.ConvexHull:
                    return ShapeType.Convexhull;
                case BoundingType.Cylinder:
                    return ShapeType.Cylinder;
                case BoundingType.Sphere:
                    return ShapeType.Sphere;
            }

            return ShapeType.Box;
        }
    }
}
