using System;
using NFluent;
using NUnit.Framework;
using pbrt.Cameras;
using pbrt.Core;
using Pbrt.Tests.Core;

namespace Pbrt.Tests.Cameras
{
    [TestFixture]
    public class PerspectiveCameraTests
    {
        private readonly Transform translate = Transform.Translate(-10, 0, 0);
        private readonly Bounds2F screenWindow = new Bounds2F(new Point2F(-1f, -1f), new Point2F(1, 1));
        private readonly Film film = new Film {FullResolution = new Point2I(640, 480)};
        private readonly Medium medium = new Medium();
        
        [Test]
        public void GenerateRay_NoFocal_Test()
        {
            var perspectiveCamera = new PerspectiveCamera(translate, screenWindow, film, medium);
            Check.That(perspectiveCamera.DxCamera.X).IsCloseTo(1.0f/32000, 1e-4);
            Check.That(perspectiveCamera.DxCamera.Y).IsEqualTo(0);
            Check.That(perspectiveCamera.DxCamera.Z).IsEqualTo(0);
            Check.That(perspectiveCamera.DyCamera.X).IsEqualTo(0);
            Check.That(perspectiveCamera.DyCamera.Y).IsCloseTo(-1.0f/24000, 1e-4);
            Check.That(perspectiveCamera.DyCamera.Z).IsEqualTo(0);
            Check.That(perspectiveCamera.A).IsEqualTo(4);
            
            // cam sample centered 
            var camSample = new CameraSample {Time = 0, PFilm = new Point2F(0,0), PLens = Point2F.Zero};
            perspectiveCamera.GenerateRay(camSample,  out var ray);

            Check.That(ray.O).Check((-10, 0, 0));
            Check.That(ray.D).Check(( -0.57735026f, 0.57735026f, 0.57735026f));

            camSample.PFilm.X = 640;
            perspectiveCamera.GenerateRay(camSample,  out ray);
            Check.That(ray.O).Check((-10, 0, 0));
            Check.That(ray.D).Check(( 0.57735026f, 0.57735026f, 0.57735026f));

            camSample.PFilm.X = 320;
            camSample.PFilm.Y = 480;
            perspectiveCamera.GenerateRay(camSample,  out ray);
            Check.That(ray.O).Check((-10, 0, 0));
            Check.That(ray.D).Check(( 0, -MathF.Sqrt(2)/2, MathF.Sqrt(2)/2));

            camSample.PFilm.X = 320;
            camSample.PFilm.Y = 240;
            perspectiveCamera.GenerateRay(camSample,  out ray);
            Check.That(ray.O).Check((-10, 0, 0));
            Check.That(ray.D).Check(( 0, 0, 1));
            
            Check.That(ray.Medium).IsSameReferenceAs(medium);
            Check.That(ray.Time).IsZero();
            Check.That(ray.TMax).Not.IsFinite();
        }
        
        [Test]
        public void GenerateRay_Focal_Test()
        {
            var perspectiveCamera = new PerspectiveCamera(translate, screenWindow, film, medium, lensRadius: 2, focalDistance: 10);
            
            // cam sample centered 
            var camSample = new CameraSample {Time = 0, PFilm = new Point2F(0,0), PLens = new Point2F(1, 1)};

            camSample.PFilm.X = 320;
            camSample.PFilm.Y = 240;
            
            perspectiveCamera.GenerateRay(camSample,  out var ray);
            Check.That(ray.O).Check((-10+MathF.Sqrt(2), +MathF.Sqrt(2), 0));
            Check.That(ray.D).IsEqualTo(new Vector3F( -MathF.Sqrt(2), -MathF.Sqrt(2), 10).Normalized());
        }
        
        [Test]
        public void GenerateRayDifferential_NoFocal_Test()
        {
            var perspectiveCamera = new PerspectiveCamera(translate, screenWindow, film, medium);
            
            // cam sample centered 
            var camSample = new CameraSample {Time = 0, PFilm = new Point2F(0,0), PLens = Point2F.Zero};
            perspectiveCamera.GenerateRayDifferential(camSample,  out var ray);

            Check.That(ray.O).Check((-10, 0, 0));
            Check.That(ray.D).Check(( -0.57735026f, 0.57735026f, 0.57735026f));
            Check.That(ray.RxOrigin).Check((-10,0,0 ));
            Check.That(ray.RyOrigin).Check((-10, 0, 0));
            Check.That(ray.RxDirection).Check(( -0.576145589f, 0.57795167f, 0.57795167f));
            Check.That(ray.RyDirection).Check((-0.57815212f, 0.575743139f, 0.57815212f));
            Check.That(ray.Medium).IsSameReferenceAs(medium);
            Check.That(ray.Time).IsZero();
            Check.That(ray.TMax).Not.IsFinite();
        }
        
        [Test]
        public void GenerateRayDifferential_Focal_Test()
        {
            var perspectiveCamera = new PerspectiveCamera(translate, screenWindow, film, medium, lensRadius: 2, focalDistance: 10);
            
            // cam sample centered 
            var camSample = new CameraSample {Time = 0, PFilm = new Point2F(0,0), PLens = new Point2F(1, 1)};

            camSample.PFilm.X = 320;
            camSample.PFilm.Y = 240;
            
            perspectiveCamera.GenerateRayDifferential(camSample,  out var ray);
            Check.That(ray.O).Check((-8.585787f, 1.4142135f, 0));
            Check.That(ray.D).IsEqualTo(new Vector3F(-0.138675049f, -0.138675049f, 0.980580688f));
            Check.That(ray.RxOrigin).Check((-8.585787f, 1.4142135f, 0f));
            Check.That(ray.RyOrigin).Check((-8.585787f, 1.4142135f, 0f));
            Check.That(ray.RxDirection).IsEqualTo(new Vector3F( -0.135667801f, -0.138733357f, 0.980993032f));
            Check.That(ray.RyDirection).IsEqualTo(new Vector3F( -0.138595387f, -0.142678767f, 0.980017424f));
        }
    }
}