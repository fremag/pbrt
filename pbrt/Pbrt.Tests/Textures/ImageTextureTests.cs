using System;
using System.IO;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Spectrums;
using pbrt.Textures;

namespace Pbrt.Tests.Textures;

[TestFixture]
public class ImageTextureTests
{
    private string img_32x32 = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAIAAAD8GO2jAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAVHSURBVEhLxdbJUlRZEAbgerYOHoIXIaJZsWHj1ghmQlBkFMWBYJAWHJhpJlFBQQXEARtHUFCk6K/I8tYt6YiOXnUubtQ9N/8/8/yZJ09lsj/t+Pg4nl++fHn16tWDBw8GBgaqq6vLyspKS0tLSkp+S5lXiz5x4MYZBDDNkyPNZnMBkqUfP37s7++/fv16cXHx5s2bbW1tVVVVZ86cqaioKC8v/z1lXi36xIEbZxBAcCTBFrSFHbCDg4O3b99y7evra25urq+vP3fuXFdXV29v7x+nzKJPHLhxBgEER5KnO7FMEurr169SWFhYkA7kxYsXu7u7Ec3Pzz9//vyvU2bRJw7cOIMAgiNBhTCYczuwKWFDGYJeuXLl6tWrt27dmpubC+oAJBb+1p8+fQoyNjY2ODjY09MDCB5acQitcgEIZ2uC+9zR0XH9+vXx8fH19fVPnz5xOjo6OqEt1C38l5eX/zyx+/fvI7137x4gOBJUHLhxzih95G6DUuA0MTEhNR7v3r378OHD58+fQ1YBBEu6QNbEuXz58vDw8OzsLLnu3r0LjiSpOfKM9vKiRESkjNyx27711dXVhw8fxuv379+xewoc/ufPn6+pqamtrZW1GLK2Do4EVdQcSUYLC6gNFIrulEHhQxTw2rVrIyMjwtjNt2/fpC8q/6amJux1dXXxFEMlnjx5Ao4EFUJuyDMk08hazX5Vle64sGBvaWnR5u3t7fa+sbGxt7en2oLRQeJMjzY2NoqBTh46AhwJKoRokWccRSxcMfJQVbpjkbv1yspKCZJ7bW1td3fXDlZWVmy/oaEBEIswAkhZkV++fAmOBBVCcD4Zx92BpBpNaE1oVaW7jOSO/dKlS7b/4sULFVMDpVNS4VGgZq2trUNDQwLLDBwJKoRokWeMFIfesRQ5+j163CYoI/cQ9+PHj1FnYQSbmZm5ceMGESQhX84g0WxIUCFEizxjbBksnHj4HIZLJehOGXTY49SEiaELtL+e0dPCcwbJf85mUSFEizxjNBpeEeDkbOdMpnpGVekeyoDlv53MRGm+f/9ev0GRlDNI/vPxcQRAi7woQLCcJFGwWPlP6/93gAQTP5Jn/EivxDN+pFeKApwuMjXD4jWBxWti6fU84CckAuSLnG5TJVUxLcFD52g7+H81bpxBAMGRFLVp+qA5ivpP25khpgVM9L6kEEV2iR0eHjrYZsPW1tajR4+WlpYAwZEUHbT0qDDiBXeInMzbt2/HjEv3aFDHD5k6Ikam6eZgGzuA4EhQFUZFetg5NY8fP56amurv7zd1jUMYZ217exudlO1G1qKidtAk4SIziC5cuEATQHAkRcMuPa4NBhkZNQaRe8MYiBtqenr62bNnOzs7tCYI6nDAa+pJNq4EQHAkReOaalbjwpGOoUhBhsKYgzf6Ozs77enOnTvmku1zc9sY1KhNOrlbd+FAgfuKqnDhmAS/XJlmHFfpSMpmpSmGjJAyP7wyWWMhiFIpL4Z/vjKVi6ymSvrSlwgPUhDHoh1E0c6ePUtcWfMZHR1Vzzdv3sC6yAw+iyFp0aWvJQwvvRz78FkKthmDmqsbSpr6ynrsPXphc3MTrx8aQSNpCg7gkTtCtMgL/+wMSB8w2iAihaIPXnT+m2CxLW0zOTmp5bWj7qKkknDjDAIIjiTulbBcgKTBhbU1KUiT1tSgDH3cLTYUhkVUprZIOXDjHFUFRxJsQZv/6xhmU4QLrRBpZLo7kA69wWJ4JebVok8cuCVVBQ9lEitI5CWecWFpYYI6io67kWJsGY2JebXoEwdunEEA0zw50mz2b/93qPr1ZMdfAAAAAElFTkSuQmCC";
    private MemoryStream stream;
    TextureMapping2D mapping = new UVMapping2D(1, 1, 0, 0);

    [SetUp]
    public void SetUp()
    {
        byte[] imgBytes = Convert.FromBase64String(img_32x32);
        stream = new MemoryStream(imgBytes, 0, imgBytes.Length);
    }
    
    [Test]
    public void BasicTest()
    {
        var imgTexture = new ImageTexture(mapping, stream, "img.png", true, 1, ImageWrap.Black, 1.23f, true);
        Check.That(imgTexture.Filename).IsEqualTo("img.png");
        Check.That(imgTexture.Mapping).IsSameReferenceAs(mapping);
        Check.That(imgTexture.DoTrilinear).IsTrue();
        Check.That(imgTexture.MaxAniso).IsEqualTo(1);
        Check.That(imgTexture.ImageWrap).IsEqualTo(ImageWrap.Black);
        Check.That(imgTexture.Scale).IsEqualTo(1.23f);
        Check.That(imgTexture.Gamma).IsTrue();
        Check.That(imgTexture.MipMap.Resolution).IsEqualTo(new Point2I(32, 32));
    }

    [Test]
    public void CacheTest()
    {
        byte[] imgBytes = Convert.FromBase64String(img_32x32);
        using var stream = new MemoryStream(imgBytes, 0, imgBytes.Length);

        TextureMapping2D mapping = new UVMapping2D(1, 1, 0, 0);
        var imgTexture = new ImageTexture(mapping, stream, "img.png", true, 1, ImageWrap.Black, 1.23f, true);

        var imgTexture2 = new ImageTexture(mapping, stream, "img.png", true, 1, ImageWrap.Black, 1.23f, true);
        Check.That(imgTexture.MipMap).IsSameReferenceAs(imgTexture2.MipMap);

        var imgTexture3 = new ImageTexture(mapping, stream, "img.png", true, 1, ImageWrap.Black, 1.23f, false);
        Check.That(imgTexture.MipMap).Not.IsSameReferenceAs(imgTexture3.MipMap);

        Check.That(ImageTexture.MipMapCache).CountIs(2);
        Check.That(ImageTexture.MipMapCache.Values).Contains(imgTexture.MipMap);
        Check.That(ImageTexture.MipMapCache.Values).Contains(imgTexture3.MipMap);
        
        ImageTexture.ClearCache();
        Check.That(ImageTexture.MipMapCache).IsEmpty();
    }

    [Test]
    public void EvaluateTest()
    {
        var imgTexture = new ImageTexture(mapping, stream, "img.png", true, 1, ImageWrap.Black, 1.23f, true);
        SurfaceInteraction si = new SurfaceInteraction
        {
            DuDx = 1.11f, DuDy = 2.22f, DvDx = 3.33f, DvDy = 4.44f,
            Uv = new Point2F(1.23f, 2.34f)
        };
        
        var spectrum = imgTexture.Evaluate(si);
        var rgb = spectrum.ToRgb();
        Check.That(rgb[0]).IsCloseTo(0.24210f, 1e-4);
        Check.That(rgb[1]).IsCloseTo(0.19143f, 1e-4);
        Check.That(rgb[2]).IsCloseTo(0.18230f, 1e-4);
    }
}