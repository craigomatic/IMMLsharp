using Imml.Runtime;
using Imml.Runtime.Services;
using IMMLSharp.Desktop.Services;
using IMMLSharp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Urho;
using Urho.Physics;

namespace IMMLSharp.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImmersiveEnvironment<Node> _Environment;
        private BasicApplication _App;

        private bool _DebugEnabled;

        public float Yaw { get; private set; }

        public float Pitch { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Imml Documents (*.imml)|*.imml";
            ofd.Multiselect = false;

            var success = ofd.ShowDialog();

            if (success.HasValue && success.Value)
            {
                Urho.Application.InvokeOnMain(async () =>
                {
                    if (_Environment != null)
                    {
                        _Environment.Dispose();
                    }

                    var elementFactory = new ImmlElementFactory();
                    var serialiser = new Imml.IO.ImmlSerialiser(elementFactory);

                    var bytes = System.IO.File.ReadAllBytes(ofd.FileName);

                    _App.Scene.CreateComponent<Octree>();
                    _App.Scene.CreateComponent<PhysicsWorld>();
                    _App.Scene.CreateComponent<DebugRenderer>();

                    var zone = _App.Scene.CreateComponent<Zone>();

                    var resourceAcquisitionService = DIContainer.Get<IResourceAcquisitionService>();

                    if (resourceAcquisitionService is LocalAndRemoteAcquisitionService)
                    {
                        (resourceAcquisitionService as LocalAndRemoteAcquisitionService).DocumentRootUri = ofd.FileName;
                    }

                    _Environment = new ImmersiveEnvironment<Node>(serialiser, resourceAcquisitionService);

                    //creates the 3d environment based on the MemoryStream passed in (the MemoryStream is the ImmlDocument representation)
                    await _Environment.CreateAsync(new MemoryStream(bytes));
                    await Urho.Application.ToMainThreadAsync();

                    _Environment.Run(_App.Scene);

                    var viewport = new Viewport(_App.Scene,
                        (_Environment.Camera as IRuntimeElement<Node>).Node.GetComponent<Urho.Camera>(), null);

                    _App.Renderer.SetViewport(0, viewport);

                    viewport.RenderPath.Append(CoreAssets.PostProcess.BloomHDR);
                    viewport.RenderPath.Append(CoreAssets.PostProcess.FXAA3);

                    zone.AmbientColor = new Urho.Color(
                        _Environment.Document.GlobalIllumination.R,
                        _Environment.Document.GlobalIllumination.G,
                        _Environment.Document.GlobalIllumination.B);
                });
            }
        }

        private void ToggleDebug_Click(object sender, RoutedEventArgs e)
        {
            _DebugEnabled = !_DebugEnabled;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _App = await UrhoSurface.Show<BasicApplication>(new ApplicationOptions
            {
                NoSound = false,
                LimitFps = false,
                ResizableWindow = true
            });

            _App.Engine.PostRenderUpdate += Engine_PostRenderUpdate;
        }

        private void Engine_PostRenderUpdate(PostRenderUpdateEventArgs obj)
        {
            if (_DebugEnabled)
            {
                _App.Renderer.DrawDebugGeometry(false);
            }

            _HandleInput(obj.TimeStep);
        }

        private void _HandleInput(float timeStep)
        {
            // Movement speed as world units per second
            const float moveSpeed = 4.0f;
            const float mouseSensitivity = .1f;

            if (_Environment?.Camera is IRuntimeElement<Node>)
            {
                var cameraNode = (_Environment.Camera as IRuntimeElement<Node>).Node;

                if (_App.Input.GetKeyDown(Urho.Key.W)) cameraNode.Translate(Vector3.UnitZ * moveSpeed * timeStep);
                if (_App.Input.GetKeyDown(Urho.Key.S)) cameraNode.Translate(-Vector3.UnitZ * moveSpeed * timeStep);
                if (_App.Input.GetKeyDown(Urho.Key.A)) cameraNode.Translate(-Vector3.UnitX * moveSpeed * timeStep);
                if (_App.Input.GetKeyDown(Urho.Key.D)) cameraNode.Translate(Vector3.UnitX * moveSpeed * timeStep);
                if (_App.Input.GetKeyDown(Urho.Key.Space)) cameraNode.Translate(Vector3.UnitY * moveSpeed * timeStep);
                if (_App.Input.GetKeyDown(Urho.Key.C)) cameraNode.Translate(-Vector3.UnitY * moveSpeed * timeStep);


                if (_App.Input.GetMouseButtonDown(Urho.MouseButton.Left))
                {
                    var mouseMove = _App.Input.MouseMove;

                    Yaw += mouseSensitivity * mouseMove.X;
                    Pitch += mouseSensitivity * mouseMove.Y;
                    Pitch = MathHelper.Clamp(Pitch, -90, 90);

                    cameraNode.Rotation = new Quaternion(Pitch, Yaw, 0);
                }
            }
        }
    }

    public class BasicApplication : Urho.Application
    {
        public Urho.Scene Scene { get; private set; }

        private string _CacheDir;

        public BasicApplication(ApplicationOptions options) : base(options)
        {
            _CacheDir = System.IO.Path.GetTempPath() + "imml";

            if (!System.IO.Directory.Exists(_CacheDir))
            {
                System.IO.Directory.CreateDirectory(_CacheDir);
            }
        }

        protected override void Setup()
        {
            base.Setup();

            this.Scene = new Urho.Scene();
        }
        protected override void Start()
        {
            base.Start();

            DIContainer.Register<Urho.Resources.ResourceCache, Urho.Resources.ResourceCache>(new Urho.Resources.ResourceCache());
            DIContainer.Register<ICacheService, CacheService>(new CacheService(_CacheDir, DIContainer.Get<Urho.Resources.ResourceCache>()));
            DIContainer.Register<IResourceAcquisitionService, LocalAndRemoteAcquisitionService>(new LocalAndRemoteAcquisitionService(DIContainer.Get<ICacheService>()));

            ResourceCache.AutoReloadResources = true;
            Renderer.HDRRendering = true;
        }
    }
}
