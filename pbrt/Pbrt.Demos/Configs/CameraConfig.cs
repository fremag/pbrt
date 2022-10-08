using pbrt.Core;

namespace Pbrt.Demos.Configs;

public class CameraConfig
{
    public Point3F Position  { get; set; }  = new Point3F(0, 2, -2);
    public Point3F LookAt  { get; set; } = new Point3F(0f, 0f, 0f);
    public int Width  { get; set; } = 640;
    public int Height  { get; set; } = 480;
    public float FoV  { get; set; } = 90f;
    public float LensRadius  { get; set; } = 0f;
    public float FocalDistance  { get; set; } = 1e6f;
    public float ShutterOpen { get; set; } = 0f;
    public float ShutterClose { get; set; } = 1f;
}