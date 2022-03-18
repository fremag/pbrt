using NFluent;
using NUnit.Framework;
using pbrt.Textures;

namespace Pbrt.Tests.Textures
{
    [TestFixture]
    public class ConstantTextureTests
    {
        [Test]
        public void EvaluateTest()
        {
            var texture = new ConstantTexture<float>(1.23f);
            Check.That(texture.Evaluate(null)).IsEqualTo(1.23f);
        }
    }
}