using System;
using System.Linq;
using pbrt.Core;
using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;
using pbrt.Lights;
using pbrt.Materials;
using pbrt.Shapes;
using pbrt.Spectrums;
using pbrt.Textures;

namespace Pbrt.Demos.Demos
{
    public class CornellBoxDemo : AbstractDemo
    {
        public override string FileName => "CornellBox.png";

        public CornellBoxDemo() : base("Cornell Box")
        {
            CameraConfig = new CameraConfig
            {
                Camera = Configs.Camera.Perspective,
                Config = new PerspectiveCameraConfig
                {
                    Position = (-278, 273, -400),
                    LookAt = (-278, 273, 0),
                    FocalDistance = 0.035f,
                    Up = (0, 1, 0)
                }
            };

            SamplerConfig = new SamplerConfig
            {
                Sampler = Configs.Sampler.Halton,
                Config = new HaltonSamplerConfig
                {
                    SamplesPerPixel = 1 << 12,
                }
            };

            IntegratorConfig = new IntegratorConfig
            {
                Integrator = IntegratorType.Path,
                Config = new PathIntegratorConfig
                {
                    MaxDepth = 4,
                    NbThreads = Environment.ProcessorCount
                }
            };
            
            Scene = new CornellBoxScene();
        }
    }

    public class CornellBoxScene : DemoScene
    {
        readonly float[] lambdas = { 400, 404, 408, 412, 416, 420, 424, 428, 432, 436, 440, 444, 448, 452, 456, 460, 464, 468, 472, 476, 480, 484, 488, 492, 496, 500, 504, 508, 512, 516, 520, 524, 528, 532, 536, 540, 544, 548, 552, 556, 560, 564, 568, 572, 576, 580, 584, 588, 592, 596, 600, 604, 608, 612, 616, 620, 624, 628, 632, 636, 640, 644, 648, 652, 656, 660, 664, 668, 672, 676, 680, 684, 688, 692, 696, 700 };
        readonly float[] white = { 0.343f, 0.445f, 0.551f, 0.624f, 0.665f, 0.687f, 0.708f, 0.723f, 0.715f, 0.710f, 0.745f, 0.758f, 0.739f, 0.767f, 0.777f, 0.765f, 0.751f, 0.745f, 0.748f, 0.729f, 0.745f, 0.757f, 0.753f, 0.750f, 0.746f, 0.747f, 0.735f, 0.732f, 0.739f, 0.734f, 0.725f, 0.721f, 0.733f, 0.725f, 0.732f, 0.743f, 0.744f, 0.748f, 0.728f, 0.716f, 0.733f, 0.726f, 0.713f, 0.740f, 0.754f, 0.764f, 0.752f, 0.736f, 0.734f, 0.741f, 0.740f, 0.732f, 0.745f, 0.755f, 0.751f, 0.744f, 0.731f, 0.733f, 0.744f, 0.731f, 0.712f, 0.708f, 0.729f, 0.730f, 0.727f, 0.707f, 0.703f, 0.729f, 0.750f, 0.760f, 0.751f, 0.739f, 0.724f, 0.730f, 0.740f, 0.737f };
        readonly float[] green = {0.092f, 0.096f, 0.098f, 0.097f, 0.098f, 0.095f, 0.095f, 0.097f, 0.095f, 0.094f, 0.097f, 0.098f, 0.096f, 0.101f, 0.103f, 0.104f, 0.107f, 0.109f, 0.112f, 0.115f, 0.125f, 0.140f, 0.160f, 0.187f, 0.229f, 0.285f, 0.343f, 0.390f, 0.435f, 0.464f, 0.472f, 0.476f, 0.481f, 0.462f, 0.447f, 0.441f, 0.426f, 0.406f, 0.373f, 0.347f, 0.337f, 0.314f, 0.285f, 0.277f, 0.266f, 0.250f, 0.230f, 0.207f, 0.186f, 0.171f, 0.160f, 0.148f, 0.141f, 0.136f, 0.130f, 0.126f, 0.123f, 0.121f, 0.122f, 0.119f, 0.114f, 0.115f, 0.117f, 0.117f, 0.118f, 0.120f, 0.122f, 0.128f, 0.132f, 0.139f, 0.144f, 0.146f, 0.150f, 0.152f, 0.157f, 0.159f};
        readonly float[] red = {0.040f, 0.046f, 0.048f, 0.053f, 0.049f, 0.050f, 0.053f, 0.055f, 0.057f, 0.056f, 0.059f, 0.057f, 0.061f, 0.061f, 0.060f, 0.062f, 0.062f, 0.062f, 0.061f, 0.062f, 0.060f, 0.059f, 0.057f, 0.058f, 0.058f, 0.058f, 0.056f, 0.055f, 0.056f, 0.059f, 0.057f, 0.055f, 0.059f, 0.059f, 0.058f, 0.059f, 0.061f, 0.061f, 0.063f, 0.063f, 0.067f, 0.068f, 0.072f, 0.080f, 0.090f, 0.099f, 0.124f, 0.154f, 0.192f, 0.255f, 0.287f, 0.349f, 0.402f, 0.443f, 0.487f, 0.513f, 0.558f, 0.584f, 0.620f, 0.606f, 0.609f, 0.651f, 0.612f, 0.610f, 0.650f, 0.638f, 0.627f, 0.620f, 0.630f, 0.628f, 0.642f, 0.639f, 0.657f, 0.639f, 0.635f, 0.642f};

        private Spectrum spectrumRed;
        private Spectrum spectrumGreen;
        private Spectrum spectrumWhite;

        // http://www.graphics.cornell.edu/online/box/data.html
        public CornellBoxScene()
        {
            InitSpectrums();
            CornellBoxFloor();
            CornelBoxCeiling();
            BackWall();

            RightWall();
            LeftWall();
            
            ShortBlock();
            TallBlock();
            var light = new[]
            {
                343.0, 548.8, 332.0,
                343.0, 548.8, 227.0,
                213.0, 548.8, 227.0,
                213.0, 548.8, 332.0
            };

            var c = 10;
            var spectrumLight = Spectrum.FromSampledSpectrum(new[] { 400.0f, 500.0f, 600.0f, 700.0f }, new[] { c*0.0f, c*8.0f, c*15.6f, c*18.4f }, 4);
            Square(spectrumLight, Transform.Translate(0, 0, 0), light);

            var lightMesh = SquareMesh(Transform.Translate(0, -0, 0), light);
            foreach (var triangle in lightMesh.GetTriangles())
            {
                var areaLight = new DiffuseAreaLight(Transform.Translate(0, 0, 0), null, spectrumLight, 10, triangle);
                AllLights.Add(areaLight);
            }
            
            var lightToWorld = Translate(-278, 540, 280) * RotateX(-90);
            IShape shape = new Disk(lightToWorld, 50f);
            var diskLight = new DiffuseAreaLight(lightToWorld, DefaultMediumInterface, new Spectrum(10f), 1, shape);
            //AllLights.Add(diskLight);
            
            //PointLight(-278, 230, 208, MathF.Pow(10, 4) );
//            Floor();
        }

        private void LeftWall()
        {
            // left wall 
            Square(spectrumRed,
                552.8, 0.0, 0.0,
                549.6, 0.0, 559.2,
                556.0, 548.8, 559.2,
                556.0, 548.8, 0.0);
        }

        private void RightWall()
        {
            // right wall
            Square(spectrumGreen,
                0.0, 0.0, 559.2,
                0.0, 0.0, 0.0,
                0.0, 548.8, 0.0,
                0.0, 548.8, 559.2);
        }

        private void BackWall()
        {
            // back wall
            Square(spectrumWhite, 
                549.6, 0.0, 559.2,
                0.0, 0.0, 559.2,
                0.0, 548.8, 559.2,
                556.0, 548.8, 559.2);
        }

        private void ShortBlock()
        {
            // Short block
            Square(spectrumWhite,
                130.0, 165.0, 65.0,
                82.0, 165.0, 225.0,
                240.0, 165.0, 272.0,
                290.0, 165.0, 114.0);

            Square(spectrumWhite,
                290.0, 0.0, 114.0,
                290.0, 165.0, 114.0,
                240.0, 165.0, 272.0,
                240.0, 0.0, 272.0);

            Square(spectrumWhite,
                130.0, 0.0, 65.0,
                130.0, 165.0, 65.0,
                290.0, 165.0, 114.0,
                290.0, 0.0, 114.0);

            Square(spectrumWhite,
                82.0, 0.0, 225.0,
                82.0, 165.0, 225.0,
                130.0, 165.0, 65.0,
                130.0, 0.0, 65.0);

            Square(spectrumWhite,
                240.0, 0.0, 272.0,
                240.0, 165.0, 272.0,
                82.0, 165.0, 225.0,
                82.0, 0.0, 225.0);            
        }

        private void CornelBoxCeiling()
        {
            // ceiling
            Square(spectrumWhite, 
                556.0, 548.8, 0.0,
                556.0, 548.8, 559.2,
                0.0, 548.8, 559.2,
                0.0, 548.8, 0.0);
        }

        private void CornellBoxFloor()
        {
            Square(spectrumWhite,
                552.8f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 559.2f,
                549.6f, 0.0f, 559.2f
            );
        }

        private void InitSpectrums()
        {
            spectrumWhite = Spectrum.FromSampledSpectrum(lambdas, white, lambdas.Length);
            spectrumGreen = Spectrum.FromSampledSpectrum(lambdas, green, lambdas.Length);
            spectrumRed = Spectrum.FromSampledSpectrum(lambdas, red, lambdas.Length);
        }

        private void TallBlock()
        {
            // Tall block
            Square(spectrumWhite,
                423.0, 330.0, 247.0,
                265.0, 330.0, 296.0,
                314.0, 330.0, 456.0,
                472.0, 330.0, 406.0);

            Square(spectrumWhite,
                423.0,   0.0, 247.0,
                423.0, 330.0, 247.0,
                472.0, 330.0, 406.0,
                472.0,   0.0, 406.0);

            Square(spectrumWhite,
                472.0, 0.0, 406.0,
                472.0, 330.0, 406.0,
                314.0, 330.0, 456.0,
                314.0, 0.0, 456.0);
            
            Square(spectrumWhite,
                314.0,   0.0, 456.0,
                314.0, 330.0, 456.0,
                265.0, 330.0, 296.0,
                265.0,   0.0, 296.0);

            Square(spectrumWhite,
                265.0, 0.0, 296.0,
                265.0, 330.0, 296.0,
                423.0, 330.0, 247.0,
                423.0, 0.0, 247.0);
        }

        public void Square(Spectrum spectrum, params double[] v)
        {
            Square(spectrum, Transform.Translate(0, 0, 0), v);
        }
        public void Square(Spectrum spectrum, Transform transform, params double[] v)
        {
            var points = Enumerable.Range(0, v.Length / 3).Select(i => new Point3F(-(float)v[3 * i], (float)v[3 * i+1], (float)v[3 * i+2])).ToArray();
            Square(spectrum, transform, points);
        }

        public TriangleMesh SquareMesh(params double[] v)
        {
            return SquareMesh(Transform.Translate(0, 0, 0), v);
        }
        
        public TriangleMesh SquareMesh(Transform transform, params double[] v)
        {
            var points = Enumerable.Range(0, v.Length / 3).Select(i => new Point3F(-(float)v[3 * i], (float)v[3 * i+1], (float)v[3 * i+2])).ToArray();
            return SquareMesh(transform, points);
        }

        public TriangleMesh SquareMesh(params Point3F[] points)
        {
            return SquareMesh(Transform.Translate(0, 0, 0), points);
        }
        
        public TriangleMesh SquareMesh(Transform transform, params Point3F[] points)
        {
            var indices = new[] { 0, 1, 2, 0, 2, 3};
            var squareMesh = new TriangleMesh(transform, 2, indices, 4, points);
            return squareMesh;
        }
        
        public void Square(Spectrum spectrum, Transform transform, params Point3F[] points)
        {
            var squareMesh = SquareMesh(transform, points);
            var matFloor = new MatteMaterial(new ConstantTexture<Spectrum>(spectrum), new ConstantTexture<float>(20), null);
            var triangles = BuildTriangles(squareMesh, matFloor);
            AddPrimitives(triangles);
        }
    }
}