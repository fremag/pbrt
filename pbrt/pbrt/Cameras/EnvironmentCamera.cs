using System;
using pbrt.Core;

namespace pbrt.Cameras
{
    public class EnvironmentCamera : AbstractCamera 
    {
        public EnvironmentCamera(Transform cameraToWorld, Film film, Medium medium, float shutterOpen=0, float shutterClose=1f )
            : base(cameraToWorld, shutterOpen, shutterClose, film, medium) 
        {
            
        }
        
        public override float GenerateRay(CameraSample sample, out Ray ray) 
        {
            // Compute environment camera ray direction 
            float theta = MathF.PI * sample.PFilm.Y / Film.FullResolution.Y;
            float phi = 2 * MathF.PI * sample.PFilm.X / Film.FullResolution.X;
            Vector3F dir = new Vector3F(MathF.Sin(theta) * MathF.Cos(phi), MathF.Cos(theta), MathF.Sin(theta) * MathF.Sin(phi));
            
            ray = new Ray(Point3F.Zero, dir, float.PositiveInfinity, MathUtils.Lerp(sample.Time, ShutterOpen, ShutterClose))
            {
                Medium = Medium
            };
            
            ray = CameraToWorld.Apply(ray);
            
            return 1;
        }        
    }
}