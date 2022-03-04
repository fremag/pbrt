using pbrt.Core;

namespace pbrt.Cameras
{
    public abstract class ProjectiveCamera : AbstractCamera
    {
        public Bounds2F ScreenWindow { get; }

        public Transform CameraToScreen { get; set; }
        public Transform RasterToCamera { get; set; }
        public Transform ScreenToRaster { get; set; }
        public Transform RasterToScreen { get; set; }
        public float LensRadius { get; set; }
        public float FocalDistance { get; set; }
        
        public ProjectiveCamera(Transform cameraToWorld,
        Transform cameraToScreen, Bounds2F screenWindow,
            float shutterOpen, float shutterClose, 
            float lensRadius, float focalDistance, Film film, Medium medium) 
            : base(cameraToWorld, shutterOpen, shutterClose, film, medium)
        {
            CameraToScreen = cameraToScreen;
            ScreenWindow = screenWindow;
            // Initialize depth of field parameters
            LensRadius = lensRadius;
            FocalDistance = focalDistance;
            
            // Compute projective camera screen transformations
            var scaleFilm = Transform.Scale(Film.FullResolution.X,  Film.FullResolution.Y, 1);
            var scaleScreenX = 1 / (screenWindow.PMax.X - screenWindow.PMin.X);
            var scaleScreenY = 1 / (screenWindow.PMin.Y - screenWindow.PMax.Y);
            var translateScreen = Transform.Translate(new Vector3F(-screenWindow.PMin.X, -screenWindow.PMax.Y, 0));
            ScreenToRaster = scaleFilm * Transform.Scale(scaleScreenX, scaleScreenY, 1) * translateScreen;
            RasterToScreen = ScreenToRaster.Inverse();            
            RasterToCamera = CameraToScreen.Inverse() * RasterToScreen;
        }
    }
}