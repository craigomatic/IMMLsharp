using Imml.Runtime;
using Imml.Runtime.Services;
using IMMLSharp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;
using Urho.Physics;
using Urho.SharpReality;
using Windows.ApplicationModel.Core;

namespace IMMLSharp
{
    internal class Program
    {
        [MTAThread]
        static void Main()
        {
            var appViewSource = new UrhoAppViewSource<HelloWorldApplication>();
            appViewSource.UrhoAppViewCreated += OnViewCreated;
            CoreApplication.Run(appViewSource);
        }

        static void OnViewCreated(UrhoAppView view)
        {
            view.WindowIsSet += View_WindowIsSet;
        }

        static void View_WindowIsSet(Windows.UI.Core.CoreWindow coreWindow)
        {
            // you can subscribe to CoreWindow events here

        }
    }

    public class HelloWorldApplication : StereoApplication
    {
        private ImmersiveEnvironment<Node> _ImmersiveEnvironment;

        public HelloWorldApplication(ApplicationOptions opts) : base(opts) { }

        protected override void OnUpdate(float timeStep)
        {
            base.OnUpdate(timeStep);
        }

        protected override void Setup()
        {
            base.Setup();

            //setup DI
            var cacheDir = Windows.Storage.ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("Imml", Windows.Storage.CreationCollisionOption.OpenIfExists).GetResults();

            DIContainer.Register<Urho.Resources.ResourceCache, Urho.Resources.ResourceCache>(new Urho.Resources.ResourceCache());
            DIContainer.Register<ICacheService, CacheService>(new CacheService(cacheDir, DIContainer.Get<Urho.Resources.ResourceCache>()));
            DIContainer.Register<IResourceAcquisitionService, ResourceAcquisitionService>(new ResourceAcquisitionService(DIContainer.Get<ICacheService>()));

            EnableGestureManipulation = true;
            EnableGestureTapped = true;

            var elementFactory = new ImmlElementFactory();
            var serialiser = new Imml.IO.ImmlSerialiser(elementFactory);

            var sceneData = Task.Run(() => EmbeddedResourceHelper.GetMemoryStream("ms-appx:///Data/scene.imml")).Result;

            _ImmersiveEnvironment = new ImmersiveEnvironment<Node>(serialiser, DIContainer.Get<IResourceAcquisitionService>());

            Task.Run(() => _ImmersiveEnvironment.CreateAsync(sceneData)).Wait();
        }

        protected override void Start()
        {
            base.Start();

            this.Scene.CreateComponent<Octree>();
            this.Scene.CreateComponent<PhysicsWorld>();

            _ImmersiveEnvironment.Run(this.Scene);
        }

        protected override void Stop()
        {
            base.Stop();

            _ImmersiveEnvironment.Dispose();
        }

        //public override void OnGestureTapped() => _ImmersiveEnvironment.HandleGesture(EventType.Tap);

        //public override void OnGestureDoubleTapped() => _ImmersiveEnvironment.HandleGesture(EventType.DoubleTap);
    }
}
