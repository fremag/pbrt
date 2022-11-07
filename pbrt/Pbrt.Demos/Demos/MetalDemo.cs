using System;
using System.IO;
using System.Linq;
using pbrt.Core;
using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;
using pbrt.Integrators;
using pbrt.Lights;
using pbrt.Materials;
using pbrt.Shapes;
using pbrt.Spectrums;
using pbrt.Textures;

namespace Pbrt.Demos.Demos
{
    public class MetalDemo : AbstractDemo
    {
        public override string FileName => $"metal.png";

        public MetalDemo() : base($"Metal")
        {
            CameraConfig = new CameraConfig
            {
                Config = new PerspectiveCameraConfig
                {
                    Position = (-0f, 3, -2),
                    LookAt = (0, 0, 1)
                }
            };

            SamplerConfig = new SamplerConfig
            {
                Sampler = Configs.Sampler.Halton,
                Config = new HaltonSamplerConfig { SamplesPerPixel = 16 }
            };

            IntegratorConfig = new IntegratorConfig
            {
                Integrator = IntegratorType.DirectLighting,
                Config = new DirectLightingConfig
                {
                    Strategy = LightStrategy.UniformSampleAll
                }
            };
            Scene = new MetalScene(1);
        }
    }

    public class MetalScene : DemoScene
    {
        public ConstantTexture<Spectrum> ReadSpectrum(string spectrumPath)
        {
            using var stream = GetResource(spectrumPath);
            using var reader = new StreamReader(stream);
            var lines = reader.ReadToEnd()
                .Split(Environment.NewLine)
                .Where(line => ! string.IsNullOrEmpty(line))
                .Select(line => line.Split(' '))
                .ToArray();
            var waveLengths = lines.Select(items => float.Parse(items[0])).ToArray();
            var values = lines.Select(items => float.Parse(items[1])).ToArray();
            var spectrum = Spectrum.FromSampledSpectrum(waveLengths, values, waveLengths.Length);
            return new ConstantTexture<Spectrum>(spectrum);
        } 
        
        public MetalScene(int nbSamples)
        {
            Sphere(0, 2f, 0, 0.5f, GetMetalMaterial("Al", 0));
            Sphere(-1, 2f, 0, 0.5f, GetMetalMaterial("Au"));
            Sphere(1, 2f, 0, 0.5f, GetMetalMaterial("Ag", 0.02f));
            Floor();

            var lightToWorld = Translate(tY: 60) * RotateX(90);
            IShape shape = new Disk(lightToWorld, 10f);
            var light = new DiffuseAreaLight(lightToWorld, DefaultMediumInterface, new Spectrum(20f), nbSamples, shape);
            PointLight(-5, 1, -5, 10f);
            AllLights.Add(light);
        }

        private IMaterial GetMetalMaterial(string name, float roughnessValue = 0.01f)
        {
            var eta = ReadSpectrum($@"Pbrt.Demos.Metals.{name}.eta.spd");
            var k = ReadSpectrum($@"Pbrt.Demos.Metals.{name}.k.spd");
            
            Texture<float> roughness = new ConstantTexture<float>(roughnessValue);
            Texture<float> uRoughness = null;
            Texture<float> vRoughness = null;
            Texture<float> bumpMap = null;
            bool remapRoughness = true;
            var material = new MetalMaterial(eta, k, roughness, uRoughness, vRoughness, bumpMap, remapRoughness);
            return material;
        }
    }
}