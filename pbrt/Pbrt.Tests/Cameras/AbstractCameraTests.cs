using System;
using NFluent;
using NUnit.Framework;
using pbrt.Cameras;
using pbrt.Core;
using pbrt.Films;
using pbrt.Media;

namespace Pbrt.Tests.Cameras
{
    // not very useful at the moment, just for code coverage
    public class AbstractCameraTests
    {
        private class DummyCamera : AbstractCamera
        {
            public DummyCamera(Transform cameraToWorld, float shutterOpen, float shutterClose, Film film, Medium medium) : base(cameraToWorld, shutterOpen, shutterClose, film, medium)
            {
            }

            public override float GenerateRay(CameraSample sample, out Ray ray)
            {
                ray = new Ray {O = new Point3F(sample.PFilm.X, sample.PFilm.Y, 0)};
                if (Math.Abs(sample.PFilm.X - 1) < float.Epsilon && sample.PFilm.Y == 0)
                {
                    return 0;
                }
                if (Math.Abs(sample.PFilm.X - 2) < float.Epsilon && sample.PFilm.Y == 0)
                {
                    return 1;
                }
                ray = new Ray {O = new Point3F(sample.PFilm.X, sample.PFilm.Y, 0)};
                return 0;
            }
        }

        [Test]
        public void GenerateRayDifferentialTest()
        {
            Transform cameraToWorld = Transform.Translate(-10, 0, 0);
            float shutterOpen = 0f;
            float shutterClose = 1f;
            Film film = new Film(640, 480);
            Medium medium = HomogeneousMedium.Default();

            var cam = new DummyCamera(cameraToWorld, shutterOpen, shutterClose, film, medium);
            var camSample = new CameraSample { Time = 0, PFilm = new Point2F(0, 0), PLens = Point2F.Zero };
            var result = cam.GenerateRayDifferential(camSample, out var ray);
            Check.That(result).IsZero();
            Check.That(ray.O.X).IsEqualTo(0);
            Check.That(ray.O.Y).IsEqualTo(0);

            camSample = new CameraSample { Time = 0, PFilm = new Point2F(1, 0), PLens = Point2F.Zero };
            result = cam.GenerateRayDifferential(camSample, out ray);
            Check.That(result).IsZero();
            Check.That(ray.O.X).IsEqualTo(1);
            Check.That(ray.O.Y).IsEqualTo(0);
        }
    }
}