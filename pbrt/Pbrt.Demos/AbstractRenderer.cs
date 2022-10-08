using pbrt.Cameras;
using pbrt.Core;
using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;
using pbrt.Films;
using pbrt.Integrators;
using pbrt.Media;
using pbrt.Samplers;

namespace Pbrt.Demos
{
    public abstract class AbstractRenderer
    {
        public CameraConfig CameraConfig { get; set; }
        public SamplerConfig SamplerConfig { get; set; }
        public IntegratorConfig IntegratorConfig { get; set; }
        
        public AbstractCamera Camera { get; protected set; }
        public AbstractSampler Sampler { get; protected set;}
        public Integrator Integrator { get; protected set;}
        public DemoScene Scene { get; protected set;}

        public abstract string FileName { get; }
        public string Text { get; protected set; }

        protected AbstractRenderer()
        {
            
        }

        public virtual void Init()
        {
            if (CameraConfig != null)
            {
                Camera = GetCam(CameraConfig.Position, CameraConfig.LookAt, CameraConfig.Width, CameraConfig.Height, CameraConfig.FoV, CameraConfig.LensRadius, CameraConfig.FocalDistance, CameraConfig.ShutterOpen, CameraConfig.ShutterClose);
            }

            if (SamplerConfig != null)
            {
                Sampler = SamplerConfig.Config.BuildSampler(Camera.Film);
            }
            
            Scene.Init();

            if (IntegratorConfig != null)
            {
                Integrator = IntegratorConfig.Config.BuildIntegrator(Sampler, Camera);
            }
            
            Integrator.Preprocess(Scene, Sampler);
        }
        
        protected AbstractRenderer(string text)
        {
            Text = text;
        }

        protected AbstractCamera GetOrthoCam(Point3F position, int width = 640, int height = 480) => GetOrthoCam(position, Point3F.Zero, width , height);
        protected AbstractCamera GetOrthoCam(Point3F position, Point3F lookAt, int width = 640, int height=480 )
        {
            Film film = new Film(width, height);
            var camera = CreateOrthographicCamera(position, film, lookAt);
            return camera;
        }

        protected AbstractCamera GetCam(Point3F position, Point3F lookAt, int width = 640, int height=480,
            float fov = 90f, 
            float lensRadius=0f, float focalDistance=1e6f, 
            float shutterOpen = 0f, float shutterClose = 1f)
        {
            Film film = new Film(width, height);
            var camera = CreatePerspectiveCamera(position, film, lookAt, fov, lensRadius, focalDistance, shutterOpen, shutterClose);
            return camera;
        }

        public static AbstractCamera CreateOrthographicCamera(Point3F position, Film film, Point3F lookAt)
        {
            var ratio = (float)film.FullResolution.X / film.FullResolution.Y;

            var cameraToWorld = Transform.LookAt(position, lookAt, new Vector3F(0, 1, 0)).Inverse();
            var screenWindow = new Bounds2F(new Point2F(-ratio, -1), new Point2F(ratio, 1));
            Medium medium = HomogeneousMedium.Default();
            return new OrthographicCamera(cameraToWorld, screenWindow, film, medium);
        }
        
        public static AbstractCamera CreatePerspectiveCamera(Point3F position, Film film, Point3F lookAt, float fov = 90f, 
            float lensRadius=0f, float focalDistance=1e6f, 
            float shutterOpen = 0f, float shutterClose = 1f
        )
        {
            var ratio = (float)film.FullResolution.X / film.FullResolution.Y;

            var cameraToWorld = Transform.LookAt(position, lookAt, new Vector3F(0, 1, 0)).Inverse();
            var screenWindow = new Bounds2F(new Point2F(-ratio, -1), new Point2F(ratio, 1));
            Medium medium = HomogeneousMedium.Default();
            return new PerspectiveCamera(cameraToWorld, screenWindow, film, medium, fov, lensRadius, focalDistance, shutterOpen, shutterClose);
        }
    }
}