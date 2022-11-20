using pbrt.Cameras;
using pbrt.Core;
using pbrt.Films;
using pbrt.Media;

namespace Pbrt.Demos.Configs;

public enum Camera
{
    Orthographic,
    Perspective
}

public class CameraConfig
{
    private Camera camera = Camera.Perspective;

    public Camera Camera
    {
        get => camera;
        set
        {
            camera = value;
            switch (camera)
            {
                case Camera.Orthographic:
                    Config = new OrthographicCameraConfig();
                    break;
                case Camera.Perspective:
                    Config = new PerspectiveCameraConfig();
                    break;
            }
        }
    }

    public AbstractCameraConfig Config { get; set; }
}

public abstract class AbstractCameraConfig
{
    public Point3F Position { get; set; } = new Point3F(0, 2, -2);
    public Point3F LookAt { get; set; } = new Point3F(0f, 0f, 0f);
    public int Width { get; set; } = 640;
    public int Height { get; set; } = 480;

    public abstract AbstractCamera BuildCamera();
}

public class OrthographicCameraConfig : AbstractCameraConfig
{
    protected AbstractCamera GetOrthoCam(Point3F position, int width = 640, int height = 480) => GetOrthoCam(position, Point3F.Zero, width, height);

    protected AbstractCamera GetOrthoCam(Point3F position, Point3F lookAt, int width = 640, int height = 480)
    {
        Film film = new Film(width, height);
        var camera = CreateOrthographicCamera(position, film, lookAt);
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

    public override AbstractCamera BuildCamera() => GetOrthoCam(Position, LookAt, Width, Height);
}

public class PerspectiveCameraConfig : AbstractCameraConfig
{
    public float FoV { get; set; } = 90f;
    public float LensRadius { get; set; } = 0f;
    public float FocalDistance { get; set; } = 1e6f;
    public float ShutterOpen { get; set; } = 0f;
    public float ShutterClose { get; set; } = 1f;
    public Vector3F Up { get; set; } = new Vector3F(0, 1, 0);

    protected AbstractCamera GetCam(Point3F position, Point3F lookAt, Vector3F up, int width = 640, int height = 480,
        float fov = 90f,
        float lensRadius = 0f, float focalDistance = 1e6f,
        float shutterOpen = 0f, float shutterClose = 1f)
    {
        Film film = new Film(width, height);
        var camera = CreatePerspectiveCamera(position, film, lookAt, up, fov: fov, lensRadius: lensRadius, focalDistance: focalDistance, shutterOpen: shutterOpen, shutterClose: shutterClose);
        return camera;
    }

    public static AbstractCamera CreatePerspectiveCamera(Point3F position, Film film, Point3F lookAt, Vector3F up, float fov = 90f,
        float lensRadius = 0f, float focalDistance = 1e6f,
        float shutterOpen = 0f, float shutterClose = 1f)
    {
        var ratio = (float)film.FullResolution.X / film.FullResolution.Y;

        var cameraToWorld = Transform.LookAt(position, lookAt, up).Inverse();
        var screenWindow = new Bounds2F(new Point2F(-ratio, -1), new Point2F(ratio, 1));
        Medium medium = HomogeneousMedium.Default();
        return new PerspectiveCamera(cameraToWorld, screenWindow, film, medium, fov, lensRadius, focalDistance, shutterOpen, shutterClose);
    }

    public override AbstractCamera BuildCamera() => GetCam(Position, LookAt, Up, width: Width, height: Height, fov: FoV, lensRadius: LensRadius, focalDistance: FocalDistance, shutterOpen: ShutterOpen, shutterClose: ShutterClose);
}