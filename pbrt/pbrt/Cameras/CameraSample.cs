using pbrt.Core;

namespace pbrt.Cameras
{
    public struct CameraSample
    {
        public Point2F PFilm { get; set; }
        public Point2F PLens { get; set; }
        public float Time { get; set; }        
    }
}