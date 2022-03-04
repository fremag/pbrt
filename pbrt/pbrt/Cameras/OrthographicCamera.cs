using pbrt.Core;

namespace pbrt.Cameras
{
    public class OrthographicCamera : ProjectiveCamera
    {
        public Vector3F DxCamera { get; set; }
        public Vector3F DyCamera { get; set; }

        public OrthographicCamera(Transform cameraToWorld, Bounds2F screenWindow,
            Film film, Medium medium,
            float lensRadius=0f, float focalDistance=1e6f, 
            float shutterOpen = 0f, float shutterClose = 1f)
            : base(cameraToWorld, Orthographic(0, 1), screenWindow, shutterOpen, shutterClose, lensRadius, focalDistance, film, medium)
        {
            DxCamera = RasterToCamera.Apply(new Vector3F(1, 0, 0));
            DyCamera = RasterToCamera.Apply(new Vector3F(0, 1, 0));
        }

        public static Transform Orthographic(float zNear, float zFar)
        {
            var translate = Transform.Translate(new Vector3F(0, 0, -zNear));
            var scale = Transform.Scale(1, 1, 1 / (zFar - zNear));
            return scale * translate;
        }

        public override float GenerateRay(CameraSample sample, out Ray ray)
        {
            // Compute raster and camera sample positions 
            Point3F pFilm = new Point3F(sample.PFilm.X, sample.PFilm.Y, 0);
            Point3F pCamera = RasterToCamera.Apply(pFilm);

            ray = new Ray(pCamera, new Vector3F(0, 0, 1));
            // Modify ray for depth of field 
            if (LensRadius > 0)
            {
                // Sample point on lens
                Point2F pLens = LensRadius * MathUtils.ConcentricSampleDisk(sample.PLens);

                // Compute point on plane of focus 
                float ft = FocalDistance / ray.D.Z;
                Point3F pFocus = ray.At(ft);

                // Update ray for effect of lens 
                ray.O = new Point3F(pLens.X, pLens.Y, 0);
                ray.D = (pFocus - ray.O).Normalized();
            }

            ray.Time = MathUtils.Lerp(sample.Time, ShutterOpen, ShutterClose);
            ray.Medium = Medium;
            ray = CameraToWorld.Apply(ray);
            return 1;
        }

        public override float GenerateRayDifferential(CameraSample sample, out RayDifferential ray)
        {
            // Compute main orthographic viewing ray 
            // Compute raster and camera sample positions>> 
            Point3F pFilm = new Point3F(sample.PFilm.X, sample.PFilm.Y, 0);
            Point3F pCamera = RasterToCamera.Apply(pFilm);
            ray = new RayDifferential(pCamera, new Vector3F(0, 0, 1));
            //Modify ray for depth of field

            //Compute ray differentials for OrthographicCamera 
            if (LensRadius > 0)
            {
                // Compute OrthographicCamera ray differentials accounting for lens 
                // Sample point on lens 
                Point2F pLens = LensRadius * MathUtils.ConcentricSampleDisk(sample.PLens);                
                float ft = FocalDistance / ray.D.Z;

                var vectorFocusZ = new Vector3F(0, 0, ft);
                Point3F pFocusX = pCamera + DxCamera + vectorFocusZ;
                ray.RxOrigin = new Point3F(pLens.X, pLens.Y, 0);
                ray.RxDirection = (pFocusX - ray.RxOrigin).Normalized();

                Point3F pFocusY = pCamera + DyCamera + vectorFocusZ;
                ray.RyOrigin = new Point3F(pLens.X, pLens.Y, 0);
                ray.RyDirection = (pFocusY - ray.RyOrigin).Normalized();
            }
            else
            {
                ray.RxOrigin = ray.O + DxCamera;
                ray.RyOrigin = ray.O + DyCamera;
                ray.RxDirection = ray.RyDirection = ray.D;
            }

            ray.Time = MathUtils.Lerp(sample.Time, ShutterOpen, ShutterClose);
            ray.HasDifferentials = true;
            ray.Medium = Medium;
            ray = CameraToWorld.Apply(ray);
            
            return 1;
        }
        
    }
}