using pbrt.Core;
using pbrt.Films;

namespace pbrt.Cameras
{
    public abstract class AbstractCamera
    {
        public Transform CameraToWorld { get; }
        public float ShutterOpen { get; }
        public float ShutterClose { get; }
        public Film Film { get; }
        public Medium Medium { get; }

        public AbstractCamera(Transform cameraToWorld, float shutterOpen, float shutterClose, Film film, Medium medium)
        {
            CameraToWorld = cameraToWorld;
            ShutterOpen = shutterOpen;
            ShutterClose = shutterClose;
            Film = film;
            Medium = medium;
        }

        public abstract float GenerateRay(CameraSample sample, out Ray ray);
        
        public virtual float GenerateRayDifferential(CameraSample sample, out RayDifferential rd) 
        {
            float wt = GenerateRay(sample, out var ray);
            rd = new RayDifferential(ray.O, ray.D);
            // Find camera ray after shifting one pixel in X the direction 
            CameraSample sshiftX = sample; // CameraSample is  a struct so = creates a copy !
            sshiftX.PFilm.X++;
            float wtx = GenerateRay(sshiftX, out var rayX);
            if (wtx == 0)
            {
                return 0;
            }
            rd.RxOrigin = rayX.O;
            rd.RxDirection = rayX.D;
            
            //Find camera ray after shifting one pixel in Y the direction 
            CameraSample sshiftY = sample; // CameraSample is  a struct so = creates a copy !
            sshiftY.PFilm.Y++;
            float wtY = GenerateRay(sshiftY, out var rayY);
            if (wtY == 0)
            {
                return 0;
            }
            rd.RyOrigin = rayY.O;
            rd.RyDirection = rayY.D;
            
            rd.HasDifferentials = true;
            return wt;    
        }        
    }
}