using System;
using NFluent;
using NUnit.Framework;
using pbrt.Cameras;
using pbrt.Core;
using Pbrt.Tests.Core;

namespace Pbrt.Tests.Cameras
{
    [TestFixture]
    public class OrthographicCameraTests
    {
        private readonly Transform translate = Transform.Translate(-10, 0, 0);
        private readonly Bounds2F screenWindow = new Bounds2F(new Point2F(-1f, -1f), new Point2F(1, 1));
        private readonly Film film = new Film {FullResolution = new Point2I(640, 480)};
        private readonly Medium medium = new Medium();
        
        [Test]
        public void GenerateRay_NoFocal_Test()
        {
            var orthoCam = new OrthographicCamera(translate, screenWindow, film, medium);
            Check.That(orthoCam.DxCamera).IsEqualTo(new Vector3F(1.0f/320,0,0));
            Check.That(orthoCam.DyCamera).IsEqualTo(new Vector3F(0,-1.0f/240,0));
            
            // cam sample centered 
            var camSample = new CameraSample {Time = 0, PFilm = new Point2F(0,0), PLens = Point2F.Zero};
            orthoCam.GenerateRay(camSample,  out var ray);

            Check.That(ray.O).Check((-11, 1, 0));
            Check.That(ray.D).Check(( 0, 0, 1));

            camSample.PFilm.X = 640;
            orthoCam.GenerateRay(camSample,  out ray);
            Check.That(ray.O).Check((-9, 1, 0));
            Check.That(ray.D).Check(( 0, 0, 1));

            camSample.PFilm.X = 320;
            camSample.PFilm.Y = 480;
            orthoCam.GenerateRay(camSample,  out ray);
            Check.That(ray.O).Check((-10, -1, 0));
            Check.That(ray.D).Check(( 0, 0, 1));

            camSample.PFilm.X = 320;
            camSample.PFilm.Y = 240;
            orthoCam.GenerateRay(camSample,  out ray);
            Check.That(ray.O).Check((-10, 0, 0));
            Check.That(ray.D).Check(( 0, 0, 1));
            
            Check.That(ray.Medium).IsSameReferenceAs(medium);
            Check.That(ray.Time).IsZero();
            Check.That(ray.TMax).Not.IsFinite();
        }
        
        [Test]
        public void GenerateRay_Focal_Test()
        {
            var orthoCam = new OrthographicCamera(translate, screenWindow, film, medium, lensRadius: 2, focalDistance: 10);
            
            // cam sample centered 
            var camSample = new CameraSample {Time = 0, PFilm = new Point2F(0,0), PLens = new Point2F(1, 1)};

            camSample.PFilm.X = 320;
            camSample.PFilm.Y = 240;
            
            orthoCam.GenerateRay(camSample,  out var ray);
            Check.That(ray.O).Check((-10+MathF.Sqrt(2), +MathF.Sqrt(2), 0));
            Check.That(ray.D).IsEqualTo(new Vector3F( -MathF.Sqrt(2), -MathF.Sqrt(2), 10).Normalized());
        }
        
        [Test]
        public void GenerateRayDifferential_NoFocal_Test()
        {
            var orthoCam = new OrthographicCamera(translate, screenWindow, film, medium);
            
            // cam sample centered 
            var camSample = new CameraSample {Time = 0, PFilm = new Point2F(0,0), PLens = Point2F.Zero};
            orthoCam.GenerateRayDifferential(camSample,  out var ray);

            Check.That(ray.O).Check((-11, 1, 0));
            Check.That(ray.D).Check(( 0, 0, 1));
            Check.That(ray.RxOrigin).Check((-11+1f/320, 1, 0));
            Check.That(ray.RyOrigin).Check((-11, 1-1f/240, 0));
            Check.That(ray.RxDirection).Check(( 0, 0, 1));
            Check.That(ray.RyDirection).Check(( 0, 0, 1));
            Check.That(ray.Medium).IsSameReferenceAs(medium);
            Check.That(ray.Time).IsZero();
            Check.That(ray.TMax).Not.IsFinite();
        }
        
        [Test]
        public void GenerateRayDifferential_Focal_Test()
        {
            var orthoCam = new OrthographicCamera(translate, screenWindow, film, medium, lensRadius: 2, focalDistance: 10);
            
            // cam sample centered 
            var camSample = new CameraSample {Time = 0, PFilm = new Point2F(0,0), PLens = new Point2F(1, 1)};

            camSample.PFilm.X = 320;
            camSample.PFilm.Y = 240;
            
            orthoCam.GenerateRayDifferential(camSample,  out var ray);
            Check.That(ray.O).Check((-10, 0, 0));
            Check.That(ray.D).IsEqualTo(new Vector3F( 0, 0, 1));
            Check.That(ray.RxOrigin).Check((-10+MathF.Sqrt(2), MathF.Sqrt(2), 0));
            Check.That(ray.RyOrigin).Check((-10+MathF.Sqrt(2), +MathF.Sqrt(2), 0));
            Check.That(ray.RxDirection).IsEqualTo(new Vector3F( 1f/320-MathF.Sqrt(2), -MathF.Sqrt(2), 10).Normalized());
            Check.That(ray.RyDirection).IsEqualTo(new Vector3F( -MathF.Sqrt(2), -1f/240-MathF.Sqrt(2), 10).Normalized());
        }
    }
}