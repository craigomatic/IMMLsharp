using Imml;
using Imml.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IMMLSharp.Unity.Scene.Controls
{
    public class Primitive : Imml.Scene.Controls.Primitive, IRuntimeElement<GameObject>
    {
        public GameObject Node { get; private set; }

        public Primitive()
        {
            this.Size = Imml.Numerics.Vector3.One;
        }

        public Task AcquireResourcesAsync()
        {
            return Task.FromResult(0);
        }

        public void ApplyLayout()
        {
            this.Node.transform.SetPositionAndRotation(
                this.WorldPosition.ToUnityVector3(),
                Quaternion.Euler(this.WorldRotation.ToUnityVector3()));

            this.Node.transform.localScale = this.Size.ToUnityVector3();
        }

        public void Dispose()
        {
            GameObject.Destroy(this.Node);
        }

        public GameObject Load(GameObject parentNode)
        {            
            switch(this.Type)
            {
                case Imml.PrimitiveType.Box:
                    {
                        this.Node = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube);                        
                        break;
                    }
                case Imml.PrimitiveType.Cone:
                    {
                        //TODO: Unity doesnt have a built in cone, supply a default mesh instead
                        this.Node = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Capsule);
                        break;
                    }
                case Imml.PrimitiveType.Cylinder:
                    {
                        this.Node = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cylinder);
                        break;
                    }
                case Imml.PrimitiveType.Plane:
                    {
                        this.Node = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Plane);
                        break;
                    }
                case Imml.PrimitiveType.Sphere:
                    {
                        this.Node = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Sphere);
                        break;
                    }
            }

            //Non-convex MeshCollider with non-kinematic Rigidbody is no longer supported since Unity 5.
            //make the Rigidbody kinematic or remove the Rigidbody component.

            var immlPhysics = this.GetPhysics();

            if (immlPhysics != null && immlPhysics.Enabled)
            {
                var rigidBody = this.Node.AddComponent<Rigidbody>();
                rigidBody.detectCollisions = true;

                if (immlPhysics.Movable)
                {
                    rigidBody.mass = immlPhysics.Weight;
                    rigidBody.centerOfMass = immlPhysics.Centre.ToUnityVector3();
                }
                else
                {
                    rigidBody.isKinematic = true;
                }

                switch (immlPhysics.Bounding)
                {
                    case BoundingType.Box:
                        {
                            this.Node.AddComponent<BoxCollider>();
                            break;
                        }
                    case BoundingType.ConvexHull:
                        {
                            var collider = this.Node.AddComponent<MeshCollider>();
                            collider.convex = true;
                            break;
                        }
                    case BoundingType.Cylinder:
                        {
                            this.Node.AddComponent<CapsuleCollider>();
                            break;
                        }
                    case BoundingType.Sphere:
                        {
                            this.Node.AddComponent<SphereCollider>();
                            break;
                        }
                }                
            }

            //just take the first material for now
            //TODO: support all material groups, textures, etc
            var material = this.GetMaterialGroup(-1).GetMaterial();
            var alpha = material.Opacity;
            var renderer = this.Node.GetComponent<MeshRenderer>();

            renderer.material.shader = UnityEngine.Shader.Find("VertexLit");

            renderer.material.SetColor("_Color", material.Diffuse.ToUnityColor(alpha));
            renderer.material.SetColor("_SpecColor", material.Specular.ToUnityColor(alpha));
            renderer.material.SetColor("_Emission", material.Emissive.ToUnityColor(alpha));
            renderer.material.SetColor("_ReflectColor", material.Ambient.ToUnityColor(alpha));
            renderer.shadowCastingMode = this.CastShadows ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = true;
            
            return this.Node;
        }
    }
}
