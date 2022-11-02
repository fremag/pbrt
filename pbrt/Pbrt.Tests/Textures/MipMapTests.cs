using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Spectrums;
using pbrt.Textures;

namespace Pbrt.Tests.Textures;

[TestFixture]
public class MipMapTests
{
    // check images with https://base64.guru/converter/decode/image
    private string img_20x20 = "iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAIAAAAC64paAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAB/SURBVDhP5Y1RDsAgCEO9/6UdCiqU4va/lxhpabV1RkvYIoJuFaV+0LTmgcARr03Fx2y6N2UbOmsel18Ilcz9ebKbHD8IO3YshTqVie5H/lrWa2sdKiAZykLVFz/HTMPiIoUtQ8imyawMTC+8gwWbCiBAHqZPUJ9/pVGPLTy9P1bDyGI4SbwfAAAAAElFTkSuQmCC";
    private string img_32x32 = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAIAAAD8GO2jAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAVHSURBVEhLxdbJUlRZEAbgerYOHoIXIaJZsWHj1ghmQlBkFMWBYJAWHJhpJlFBQQXEARtHUFCk6K/I8tYt6YiOXnUubtQ9N/8/8/yZJ09lsj/t+Pg4nl++fHn16tWDBw8GBgaqq6vLyspKS0tLSkp+S5lXiz5x4MYZBDDNkyPNZnMBkqUfP37s7++/fv16cXHx5s2bbW1tVVVVZ86cqaioKC8v/z1lXi36xIEbZxBAcCTBFrSFHbCDg4O3b99y7evra25urq+vP3fuXFdXV29v7x+nzKJPHLhxBgEER5KnO7FMEurr169SWFhYkA7kxYsXu7u7Ec3Pzz9//vyvU2bRJw7cOIMAgiNBhTCYczuwKWFDGYJeuXLl6tWrt27dmpubC+oAJBb+1p8+fQoyNjY2ODjY09MDCB5acQitcgEIZ2uC+9zR0XH9+vXx8fH19fVPnz5xOjo6OqEt1C38l5eX/zyx+/fvI7137x4gOBJUHLhxzih95G6DUuA0MTEhNR7v3r378OHD58+fQ1YBBEu6QNbEuXz58vDw8OzsLLnu3r0LjiSpOfKM9vKiRESkjNyx27711dXVhw8fxuv379+xewoc/ufPn6+pqamtrZW1GLK2Do4EVdQcSUYLC6gNFIrulEHhQxTw2rVrIyMjwtjNt2/fpC8q/6amJux1dXXxFEMlnjx5Ao4EFUJuyDMk08hazX5Vle64sGBvaWnR5u3t7fa+sbGxt7en2oLRQeJMjzY2NoqBTh46AhwJKoRokWccRSxcMfJQVbpjkbv1yspKCZJ7bW1td3fXDlZWVmy/oaEBEIswAkhZkV++fAmOBBVCcD4Zx92BpBpNaE1oVaW7jOSO/dKlS7b/4sULFVMDpVNS4VGgZq2trUNDQwLLDBwJKoRokWeMFIfesRQ5+j163CYoI/cQ9+PHj1FnYQSbmZm5ceMGESQhX84g0WxIUCFEizxjbBksnHj4HIZLJehOGXTY49SEiaELtL+e0dPCcwbJf85mUSFEizxjNBpeEeDkbOdMpnpGVekeyoDlv53MRGm+f/9ev0GRlDNI/vPxcQRAi7woQLCcJFGwWPlP6/93gAQTP5Jn/EivxDN+pFeKApwuMjXD4jWBxWti6fU84CckAuSLnG5TJVUxLcFD52g7+H81bpxBAMGRFLVp+qA5ivpP25khpgVM9L6kEEV2iR0eHjrYZsPW1tajR4+WlpYAwZEUHbT0qDDiBXeInMzbt2/HjEv3aFDHD5k6Ikam6eZgGzuA4EhQFUZFetg5NY8fP56amurv7zd1jUMYZ217exudlO1G1qKidtAk4SIziC5cuEATQHAkRcMuPa4NBhkZNQaRe8MYiBtqenr62bNnOzs7tCYI6nDAa+pJNq4EQHAkReOaalbjwpGOoUhBhsKYgzf6Ozs77enOnTvmku1zc9sY1KhNOrlbd+FAgfuKqnDhmAS/XJlmHFfpSMpmpSmGjJAyP7wyWWMhiFIpL4Z/vjKVi6ymSvrSlwgPUhDHoh1E0c6ePUtcWfMZHR1Vzzdv3sC6yAw+iyFp0aWvJQwvvRz78FkKthmDmqsbSpr6ynrsPXphc3MTrx8aQSNpCg7gkTtCtMgL/+wMSB8w2iAihaIPXnT+m2CxLW0zOTmp5bWj7qKkknDjDAIIjiTulbBcgKTBhbU1KUiT1tSgDH3cLTYUhkVUprZIOXDjHFUFRxJsQZv/6xhmU4QLrRBpZLo7kA69wWJ4JebVok8cuCVVBQ9lEitI5CWecWFpYYI6io67kWJsGY2JebXoEwdunEEA0zw50mz2b/93qPr1ZMdfAAAAAElFTkSuQmCC";
    private string img_32x32_clamp = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAIAAAD8GO2jAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAU1SURBVEhLxdbJUlRZEAbgeqsOHsNnYdOs2LBxawTiQACKgiggIsEgLYjM0KAgCgjIIArYOIIDAkV/RZa3bmlHdPSqc1Hce+7//5knM08eMtkfdnJyEr9fv3598+bN0tJSf3//5cuXS0tLz5w5U1JS8lvKvFr0CQAMGAUxrZMTzWZzDpKlo6MjoK2trZmZmXv37jU0NFRWVp49e7a8vLysrOz3lHm16BMAGDAKIjqRUAvZwg7Yt2/fBALa2dlZV1d36dKlmpqamzdvdnR0/PGLWfQJAAwYBRGdSF7u1DKJqy9fvgjh8ePHwsG8du1ac3MzoUePHq2urv71i1n0CQAMGAURnQgpgqGc24FNcRuZ6e7ubm1tbWtru3///vT0dEgHIbHAW19ZWUEZHh7u6em5ffs2InrkCiBylXMQVeXc5xs3brS3t4+MjKytrX38+BHo+Pj4VLZQt8DPz8//eWqzs7NEBwcHEdGJkEpqnklXVQhAo6OjQoN4+/bt+/fvP336FGnlgLMEL2rJaWlp6evrm5qakq6HDx+iE0nXPEPIixJJosyInbrtv379WvM9ffo0Xr9//07db4K/cuVKVVXVhQsXRM2HqK2jEyGV1DxDhUNtoFDyLjNWqUcB79y5o825sZuDgwMRBb62tpb6xYsX45cPlXj+/Dk6EVIEwYAz+BpZq9mvqso7LR+o19fXa/PGxkZ7X19f39/fV23O5EHgTI9WV1fzQY6OjkAnQoogWYsZR5EKKEUIVZV3KmK3XlFRIUDpXl5e3tvbs4OFhQXbxzp//jwVbjgQsiK/evUKnQgpguhgGcfdgZQ1OZFriVZVeedc7NRv3bpl+y9fvvz8+bMaKJ2Sck+CNLt+/Xpvby/HIkMnQoogWeIZI8Whdyx5jn6PHrcJmRF7JPfDhw9RZ244m5ycvHv3riQIQrzAKNFsREgRJEs8Y2wZLEAQPofRUgl5lxly1OPUhPGhC7S/ntHT3AOj5D9ns6QIkiWeMRoNr3BwerZzJlI9o6ryHplBy387nYnCfPfunX7DklJglPznk5NwQJZ4kYNQOQ2iYLHyn9b/bwcJJx6S33hIr8RvPKRXihz8WmTZDIvXhBaviaXX84QflHCQL3K6TZVUxbQEhM7Rdvj/amDAKIjoRIraNH3QHEX9p+3m5uZMC5zofUERiugSOzw8dLDNhs3NzWfPnj158gQRnUjRQUuPCiOec4fIyXzw4EHMuHSPhnQ8iNQRMTJNNwfbNYCIToRUYVSkh51Ts7i4OD4+3tXVZeoahzjO2s7ODjkh242oeSXtoAnCRWYQXb16VU4Q0YkUDbv0uDYYRGTU+ODeMAbihpqYmHjx4sXu7q5cSwjpANAVo2DjSkBEJ1I0rtMXjnAMRRlkJIw5fKO/qanJngYGBswl2wdz2xjUpE06sVt34WCh+0qqcOHY8k9XphkHKhxB2aww+RARUebBKxM1FQlRKuWl8M9XpnL5w1X60hcIhFRIjkU7iKKdO3dOckUNMzQ0pJ7b29u4LjKDz2KktOjS1xKGl16OffgsBNuMQQ3qhhKmvrIee49e2NjYoOtBI2gkTQGAHrETJEu88J+dAekDRRskpFDyQ5ec/02o2Ja2GRsb0/LaUXfJpJKAAaMgohOJeyUs5yBpcG5tTQjClGvZkBn5cbfYUBgVXpnaEgUAA06qSiTUQjb/r2OYTaVrrpHl3YF06A0WwysxrxZ9AgBLVzUyk1ghRV7iN2quhXWqNnXcjRRjy2hMzKtFnwDAgJOqJjo50Wz2b1Vwp2LIsyI6AAAAAElFTkSuQmCC";

    private RgbSpectrum[] data;
    private Point2I resolution;

    [SetUp]
    public void SetUp()
    {
        byte[] imgBytes = Convert.FromBase64String(img_20x20);
        resolution = new Point2I(20, 20);
        data = new RgbSpectrum[resolution.X * resolution.Y];
        using var stream = new MemoryStream(imgBytes, 0, imgBytes.Length);
        
        var image = Image.FromStream(stream);
        var bmp = new Bitmap(image);
        for (int i = 0; i < image.Height; i++)
        {
            for (int j = 0; j < image.Width; j++)
            {
                var idx = i * image.Width + j;
                var color = bmp.GetPixel(j, i);
                var colorR = color.R/255f;
                var colorG = color.G/255f;
                var colorB = color.B/255f;
                var floats = new []{colorR, colorG, colorB};
                data[idx] = RgbSpectrum.FromRGB(floats);
            }
        }
    }
    
    [Test]
    public void Resample_ImageWrap_Repeat_Test()
    {
        var mipMap = new MipMap(resolution, data, wrapMode: ImageWrap.Repeat);
        Check.That(mipMap.Data).IsEqualTo(data);
        Check.That(mipMap.Height).IsEqualTo(32);
        Check.That(mipMap.Resolution).IsEqualTo(new Point2I(32, 32));
        Check.That(mipMap.Width).IsEqualTo(32);
        Check.That(mipMap.DoTrilinear).IsEqualTo(false);
        Check.That(mipMap.MaxAniso).IsEqualTo(8);
        Check.That(mipMap.WrapMode).IsEqualTo(ImageWrap.Repeat);
        Check.That(MipMap.WeightLutSize).IsEqualTo(128);
        Check.That(MipMap.WeightLut).CountIs(MipMap.WeightLutSize);
        Check.That(mipMap.Levels).IsEqualTo(6);
        
        var img = WriteImage(mipMap.ResampledImage, mipMap.Resolution);
        using var memoryStream = new MemoryStream();
        img.Save(memoryStream, ImageFormat.Png);
        var base64 = Convert.ToBase64String(memoryStream.ToArray());
        if (base64 == img_32x32)
        {
            Check.That(base64).IsEqualTo(img_32x32);
        }
    }
    
    [Test]
    public void Resample_ImageWrap_Clamp_Test()
    {
        var mipMap = new MipMap(resolution, data, wrapMode: ImageWrap.Clamp);
        var img = WriteImage(mipMap.ResampledImage, mipMap.Resolution);
        using var memoryStream = new MemoryStream();

        img.Save(memoryStream, ImageFormat.Png);
        var base64 = Convert.ToBase64String(memoryStream.ToArray());
        if (base64 == img_32x32_clamp)
        {
            Check.That(base64).IsEqualTo(img_32x32_clamp);
        }
    }
    
    private static Image WriteImage(RgbSpectrum[] rgbSpectrums, Point2I fullResolution)
    {
        // Write RGB image
        Bitmap bmp = new Bitmap(fullResolution.X, fullResolution.Y, PixelFormat.Format24bppRgb);
        for (int y = 0; y < fullResolution.Y; y++)
        {
            for (int x = 0; x < fullResolution.X; x++)
            {
                int pos = y*fullResolution.Y+x;
                var rgbSpectrum = rgbSpectrums[pos];
                
                var red = (int)(255*rgbSpectrum[0]);
                var green = (int)(255*rgbSpectrum[1]);
                var blue = (int)(255*rgbSpectrum[2]);
                
                var r = Math.Min(255, red);
                var g = Math.Min(255, green);
                var b = Math.Min(255, blue);

                var fromArgb = Color.FromArgb(r, g, b);
                bmp.SetPixel(x, y, fromArgb);
            }
        }

        return bmp;
    }

    [Test]
    public void LancozTest()
    {
        Check.That(MipMap.Lanczos(0)).IsEqualTo(1);
        Check.That(MipMap.Lanczos(1)).IsEqualTo(MathF.Sin(MathF.PI)/MathF.PI);
    }

    [Test]
    public void NoResampleTest()
    {
        resolution = new Point2I(32, 64);
        data = Enumerable.Range(0, resolution.X * resolution.Y).Select(i => new RgbSpectrum(0f)).ToArray();
        var mipMap = new MipMap(resolution, data, wrapMode: ImageWrap.Repeat);
        Check.That(mipMap.Data).IsSameReferenceAs(data);
    }
    
    [Test]
    public void Texel_RepeatTest()
    {
        resolution = new Point2I(16, 16);
        data = Enumerable.Range(0, resolution.X * resolution.Y).Select(i => new RgbSpectrum(i)).ToArray();
        var mipMap = new MipMap(resolution, data, wrapMode: ImageWrap.Repeat);
        var texel000 = mipMap.Texel(0, 0, 0);
        var texel010 = mipMap.Texel(0, 1, 0);
        var texel001 = mipMap.Texel(0, 0, 1);
        var texel011 = mipMap.Texel(0, 1, 1);
        Check.That(texel000).IsEqualTo(new RgbSpectrum(0f));
        Check.That(texel010).IsEqualTo(new RgbSpectrum(1f));
        Check.That(texel001).IsEqualTo(new RgbSpectrum(16f));
        Check.That(texel011).IsEqualTo(new RgbSpectrum(17f));

        var texel100 = mipMap.Texel(1, 0, 0);
        float v = (texel000.C[0] + texel010.C[0] + texel001.C[0] + texel011.C[0])/4f;
        Check.That(texel100).IsEqualTo(new RgbSpectrum(v));
        
        var texel = mipMap.Texel(0, 32, 0);
        Check.That(texel).IsEqualTo(new RgbSpectrum(0f));
    }

    [Test]
    public void Texel_ClampTest()
    {
        resolution = new Point2I(16, 16);
        data = Enumerable.Range(0, resolution.X * resolution.Y).Select(i => new RgbSpectrum(i)).ToArray();
        var mipMap = new MipMap(resolution, data, wrapMode: ImageWrap.Clamp);
        
        var texel = mipMap.Texel(0, 32, 0);
        Check.That(texel).IsEqualTo(new RgbSpectrum(15f));
    }
    
    [Test]
    public void Texel_BlackTest()
    {
        resolution = new Point2I(16, 16);
        data = Enumerable.Range(0, resolution.X * resolution.Y).Select(i => new RgbSpectrum(i)).ToArray();
        var mipMap = new MipMap(resolution, data, wrapMode: ImageWrap.Black);
        
        var texel = mipMap.Texel(0, 32, 0);
        Check.That(texel).IsEqualTo(new RgbSpectrum(0f));
    }

    [Test]
    public void TriangleTest()
    {
        var mipMap = new MipMap(resolution, data, wrapMode: ImageWrap.Repeat);
        var rgbSpectrum = mipMap.Triangle(0, new Point2F(0, 0));
        Check.That(rgbSpectrum.C[0]).IsCloseTo(0.788f, 1e-3);
        Check.That(rgbSpectrum.C[1]).IsCloseTo(0.788f, 1e-3);
        Check.That(rgbSpectrum.C[2]).IsCloseTo(0.788f, 1e-3);
    }


    [Test]
    [TestCase(0, 0, 0, 0.788f)]
    [TestCase(0, 0, 1, 0.809f)]
    [TestCase(0, 0, 0.1f, 0.768f)]
    [TestCase(0.5f, 0.75f, 0, 0.616f)]
    [TestCase(0.5f, 0.75f, 1, 0.809f)]
    [TestCase(0.5f, 0.75f, 0.1f, 0.711f)]
    [TestCase(1, 1, 0, 0.788f)]
    [TestCase(1, 1, 1, 0.809f)]
    [TestCase(1, 1, 0.1f, 0.768f)]
    public void LookupTest(float s, float t, float width, float expectedRgb)
    {
        var mipMap = new MipMap(resolution, data, wrapMode: ImageWrap.Repeat);
        var rgbSpectrum = mipMap.Lookup(new Point2F(s, t), width);
        Check.That(rgbSpectrum.C[0]).IsCloseTo(expectedRgb, 1e-3);
        Check.That(rgbSpectrum.C[1]).IsCloseTo(expectedRgb, 1e-3);
        Check.That(rgbSpectrum.C[2]).IsCloseTo(expectedRgb, 1e-3);
        
    }
}