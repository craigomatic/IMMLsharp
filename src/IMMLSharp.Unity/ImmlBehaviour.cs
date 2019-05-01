using Imml;
using Imml.IO;
using Imml.Runtime;
using Imml.Runtime.Services;
using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityImml
{
    public class ImmlBehaviour : MonoBehaviour
    {
        void Start()
        {
            var serialiser = new ImmlSerialiser(new UnityElementFactory());
            var resourceAcquisitionService = new ResourceAcquisitionService(new InMemoryCache());
            var immersiveEnvironment = new ImmersiveEnvironment<GameObject>(serialiser, resourceAcquisitionService, new UnityLog());

            Debug.Log("Loading scene from resource");

            var resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("IMMLSharp.Unity.scene.imml");
            Debug.Log("Scene resource ready");

            var t = immersiveEnvironment.CreateAsync(resStream);
            t.Wait();

            immersiveEnvironment.Run(this.gameObject);

            Debug.Log("Loading complete");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
