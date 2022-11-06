using NFluent;
using NUnit.Framework;
using pbrt.Textures;

namespace Pbrt.Tests.Textures;

[TestFixture]
public class TextureInfoTests
{
    [Test]
    public void BasicTest()
    {
        TextureInfo info1 = new TextureInfo("a.png", false, 0, ImageWrap.Black, 1, true);
        TextureInfo info2 = new TextureInfo("b.png", false, 0, ImageWrap.Black, 1, true);
        TextureInfo info3 = new TextureInfo("b.png", false, 0, ImageWrap.Black, 1, true);

        Check.That(info1).Not.IsEqualTo(info2);
        Check.That(info1.Equals((object)null)).IsFalse();
        Check.That(info1.Equals(5)).IsFalse();
        Check.That(info2).IsEqualTo(info2);
        Check.That(info2).IsEqualTo(info3);
        
        Check.That(info1.GetHashCode()).Not.IsEqualTo(info2.GetHashCode());
        Check.That(info1.GetHashCode()).IsEqualTo(info1.GetHashCode());
       
    }
}