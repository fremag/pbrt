using System;
using System.Numerics;
using pbrt.Core;
using pbrt.Films;
using pbrt.Media;

namespace pbrt.Cameras
{
    public class PerspectiveCamera : ProjectiveCamera
    {
        public Vector3F DxCamera { get; set; }
        public Vector3F DyCamera { get; set; }
        public float A { get; set; }

        public PerspectiveCamera(Transform cameraToWorld, Bounds2F screenWindow,
            Film film, Medium medium,
            float fov = 90f, 
            float lensRadius=0f, float focalDistance=1e6f, 
            float shutterOpen = 0f, float shutterClose = 1f
            )
            : base(cameraToWorld, Perspective(fov, 1e-2f, 1000f), screenWindow, shutterOpen, shutterClose, lensRadius, focalDistance, film, medium)
        {
            // Compute differential changes in origin for perspective camera rays>> 
            DxCamera = RasterToCamera.Apply(new Point3F(1, 0, 0)) - RasterToCamera.Apply(Point3F.Zero);
            DyCamera = RasterToCamera.Apply(new Point3F(0, 1, 0)) - RasterToCamera.Apply(Point3F.Zero);

            // Compute image plane bounds at z=1 for PerspectiveCamera 
            Point2I res = film.FullResolution;
            Point3F pMin = RasterToCamera.Apply(Point3F.Zero);
            Point3F pMax = RasterToCamera.Apply(new Point3F(res.X, res.Y, 0));
            pMin /= pMin.Z;
            pMax /= pMax.Z;
            A = MathF.Abs((pMax.X - pMin.X) * (pMax.Y - pMin.Y));
        }

        public static Transform Perspective(float fov, float n, float f)
        {
            // Perform projective divide for perspective projection 
            Matrix4x4 persp = new Matrix4x4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, f / (f - n), -f * n / (f - n),
                0, 0, 1, 0);

            // Scale canonical perspective view to specified field of view
            float invTanAng = 1 / MathF.Tan(fov.Radians() / 2);
            return Transform.Scale(invTanAng, invTanAng, 1) * new Transform(persp);
        }

        public override float GenerateRay(CameraSample sample, out Ray ray)
        {
            // Compute raster and camera sample positions>> 
            Point3F pFilm = new Point3F(sample.PFilm.X, sample.PFilm.Y, 0);
            Point3F pCamera = RasterToCamera.Apply(pFilm);

            ray = new Ray(Point3F.Zero, (new Vector3F(pCamera)).Normalized());
            // Modify ray for depth of field 
            if (LensRadius > 0)
            {
                // Sample point on lens>> 
                Point2F pLens = LensRadius * MathUtils.ConcentricSampleDisk(sample.PLens);

                // Compute point on plane of focus>> 
                float ft = FocalDistance / ray.D.Z;
                Point3F pFocus = ray.At(ft);

                // Update ray for effect of lens>> 
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
            // Compute raster and camera sample positions
            Point3F pFilm = new Point3F(sample.PFilm.X, sample.PFilm.Y, 0);
            Point3F pCamera = RasterToCamera.Apply(pFilm);
            Vector3F dir = (new Vector3F(pCamera.X, pCamera.Y, pCamera.Z)).Normalized();
            ray = new RayDifferential(new Point3F(0, 0, 0), dir);
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

            // Compute offset rays for PerspectiveCamera ray differentials
            if (LensRadius > 0)
            {
                // Compute_PerspectiveCamera ray differentials accounting for lens
                // Sample point on lens
                Point2F pLens = LensRadius * MathUtils.ConcentricSampleDisk(sample.PLens);
                Vector3F dx = (new Vector3F(pCamera + DxCamera)).Normalized();
                float ft = FocalDistance / dx.Z;
                Point3F pFocus = new Point3F(0, 0, 0) + (ft * dx);
                ray.RxOrigin = new Point3F(pLens.X, pLens.Y, 0);
                ray.RxDirection = (pFocus - ray.RxOrigin).Normalized();

                Vector3F dy = (new Vector3F(pCamera + DyCamera)).Normalized();
                ft = FocalDistance / dy.Z;
                pFocus = new Point3F(0, 0, 0) + (ft * dy);
                ray.RyOrigin = new Point3F(pLens.X, pLens.Y, 0);
                ray.RyDirection = (pFocus - ray.RyOrigin).Normalized();
            }
            else
            {
                ray.RxOrigin = ray.O;
                ray.RyOrigin = ray.O;

                ray.RxDirection = (new Vector3F(pCamera) + DxCamera).Normalized();
                ray.RyDirection = (new Vector3F(pCamera) + DyCamera).Normalized();
            }

            ray.Time = MathUtils.Lerp(sample.Time, ShutterOpen, ShutterClose);
            ray.Medium = Medium;
            ray = CameraToWorld.Apply(ray);
            ray.HasDifferentials = true;
            return 1;
        }
    }
}