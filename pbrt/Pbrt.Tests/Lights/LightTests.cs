using NFluent;
using NSubstitute;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Lights;

namespace Pbrt.Tests.Lights
{
    [TestFixture]
    public class LightTests
    {
        [Test]
        public void BasicTest()
        {
            Transform lightToWorld = Transform.Translate(1, 0, 0);
            MediumInterface mediumInterface = new MediumInterface(null);
            int nSamples = 42;
            Light light = Substitute.For<Light>(LightFlags.Area,  lightToWorld, mediumInterface, nSamples);
            Check.That(light.Flags).IsEqualTo(LightFlags.Area);
            Check.That(light.MediumInterface).IsSameReferenceAs(mediumInterface);
            Check.That(light.NSamples).IsEqualTo(42);
            Check.That(light.LightToWorld).IsEqualTo(lightToWorld);
            Check.That(light.WorldToLight).IsEqualTo(lightToWorld.Inverse());
            light.Preprocess(null);
        }

        [Test]
        [TestCase(LightFlags.Area, false)]
        [TestCase(LightFlags.Area|LightFlags.Infinite, false)]
        [TestCase(LightFlags.Area|LightFlags.DeltaDirection, true)]
        [TestCase(LightFlags.Area|LightFlags.DeltaPosition, true)]
        public void Test(LightFlags flags, bool expected)
        {
            Check.That(Light.IsDeltaLight(flags)).IsEqualTo(expected);
        }
    }
}