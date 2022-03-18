using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Spectrums;
using pbrt.Textures;

namespace Pbrt.Tests.Textures
{
    [TestFixture]
    public class UvTextureTests
    {
        [Test]
        [TestCase(0,0,0,0,0)]
        [TestCase(1,1,0,0,0)]
        [TestCase(0.5f,0,0.580073476f,-0.00119443308f,-0.000993698835f)]
        [TestCase(0.5f,0.5f,0.5988398f, 0.47390318f, 0.006912291f)]
        public void EvaluateTest(float u, float v, float r, float g, float b)
        {
            UVMapping2D uvMap2D = new UVMapping2D(1f, 1f, 0f, 0f);
            SurfaceInteraction si = new SurfaceInteraction();
            si.DuDx = 0f;
            si.DuDy = 0f;
            si.DvDx = 0f;
            si.DvDy = 0f;
            si.Uv = new Point2F(u, v);
            
            UvTexture uvTexture = new UvTexture(uvMap2D);
            Spectrum f = uvTexture.Evaluate(si);
            var rgb = f.ToRgb();
            Check.That(rgb[0]).IsEqualTo(r);
            Check.That(rgb[1]).IsEqualTo(g);
            Check.That(rgb[2]).IsEqualTo(b);
        }
    }
}