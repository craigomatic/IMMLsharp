using Imml;
using Imml.Runtime;
using Imml.Runtime.Services;
using ImmlSharp.StereoKit;
using ImmlSharp.StereoKit.Services;
using IMMLSharp.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using SK = StereoKit;

namespace IMMLSharp.StereoKit
{
    class Program
    {
        private static ImmersiveEnvironment<SK.Model> _ImmersiveEnvironment;

        static void Main(string[] args)
        {
            var t = Task.Run(() => AsyncMain(args));
            t.Wait();
        }

        static async Task AsyncMain(string[] args)
        {
            var cacheDir = Windows.Storage.ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("Imml", Windows.Storage.CreationCollisionOption.OpenIfExists).GetResults();

            if (!SK.StereoKitApp.Initialize("IMMLSharp.StereoKit", SK.Runtime.Flatscreen))
                Environment.Exit(1);

            DIContainer.Register<ICacheService, CacheService>(new CacheService(cacheDir));
            DIContainer.Register<IResourceAcquisitionService, LocalAndRemoteAcquisitionService>(
                new LocalAndRemoteAcquisitionService(DIContainer.Get<ICacheService>()));

            var elementFactory = new ImmlElementFactory();
            var serialiser = new Imml.IO.ImmlSerialiser(elementFactory);

            var sceneData = Task.Run(() => EmbeddedResourceHelper.GetMemoryStream("ms-appx:///Data/scene.imml")).Result;

            _ImmersiveEnvironment = new ImmersiveEnvironment<SK.Model>(serialiser, DIContainer.Get<IResourceAcquisitionService>());

            await _ImmersiveEnvironment.CreateAsync(sceneData);
            
            _ImmersiveEnvironment.Run(null);

            var camPos = _ImmersiveEnvironment.Camera.Position;

            //switch the view to +z into the screen vs. SK default which is -z into the screen
            SK.Renderer.SetView(SK.Matrix.TRS(camPos.ToSKVec3(), SK.Quat.LookAt(SK.Vec3.Zero, new SK.Vec3(0, 0, 1))));

            while (SK.StereoKitApp.Step(() =>
            {
                var allRuntimeElements = _ImmersiveEnvironment.Document.Elements.AsRecursiveEnumerable().OfType<IRuntimeElement<SK.Model>>();

                foreach (var runtimeElement in allRuntimeElements)
                {
                    runtimeElement.Render();
                }
            })) ;

            _ImmersiveEnvironment.Dispose();

            SK.StereoKitApp.Shutdown();
        }
    }
}
