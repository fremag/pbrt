using System;
using NFluent;
using NUnit.Framework;
using pbrt.Cameras;
using pbrt.Core;
using Pbrt.Tests.Core;

namespace Pbrt.Tests.Cameras
{
    [TestFixture]
    public class EnvironmentCameraTests
    {
        private readonly Transform translate = Transform.Translate(-10, 0, 0);
        private readonly Film film = new Film { FullResolution = new Point2I(640, 480) };
        private readonly Medium medium = new Medium();

        [Test]
        public void GenerateRay_Test()
        {
            var envCam = new EnvironmentCamera(translate, film, medium);

            // cam sample centered 
            var camSample = new CameraSample { Time = 0, PFilm = new Point2F(0, 0), PLens = Point2F.Zero };
            envCam.GenerateRay(camSample, out var ray);

            Check.That(ray.O).Check((-10, 0, 0));
            Check.That(ray.D).Check((0, 1, 0));

            camSample.PFilm.X = 640;
            envCam.GenerateRay(camSample, out ray);
            Check.That(ray.O).Check((-10, 0, 0));
            Check.That(ray.D).Check((0, 1, 0));

            camSample.PFilm.X = 320;
            camSample.PFilm.Y = 480;
            envCam.GenerateRay(camSample, out ray);
            Check.That(ray.O).Check((-10, 0, 0));
            Check.That(ray.D).Check((8.74227766E-08f, -1f, 7.64274186E-15f));

            camSample.PFilm.X = 320;
            camSample.PFilm.Y = 240;
            envCam.GenerateRay(camSample, out ray);
            Check.That(ray.O).Check((-10, 0, 0));
            Check.That(ray.D).Check((-1f, -4.37113883E-08f, -8.74227766E-08f));

            Check.That(ray.Medium).IsSameReferenceAs(medium);
            Check.That(ray.Time).IsZero();
            Check.That(ray.TMax).Not.IsFinite();
        }


        [Test]
        public void GenerateRayDifferential_Test()
        {
            var envCam = new EnvironmentCamera(translate, film, medium);

            // cam sample centered 
            var camSample = new CameraSample { Time = 0, PFilm = new Point2F(0, 0), PLens = Point2F.Zero };
            envCam.GenerateRayDifferential(camSample, out var ray);

            Check.That(ray.O).Check((-10, 0, 0));
            Check.That(ray.D).Check((0, 1, 0));
            Check.That(ray.RxOrigin).Check((-10f, 0f, 0f));
            Check.That(ray.RyOrigin).Check((-10f, 0f, 0f));
            Check.That(ray.RxDirection).Check((0, 1, 0));
            Check.That(ray.RyDirection).Check((0.00654462259f, 0.999978602f, 6.42537561E-05f));
            Check.That(ray.Medium).IsNull();
            Check.That(ray.Time).IsZero();
            Check.That(ray.TMax).Not.IsFinite();
        }
    }
}