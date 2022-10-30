using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

    // for some reasons, gitHub actions gives another result when writing png files but appveyor / coveralls.io are ok with me  
    private string img_32x32_bis = "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";
    private string img_32x32_clamp_bis = "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";
    
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
        Check.That(mipMap.DoTri).IsEqualTo(false);
        Check.That(mipMap.MaxAniso).IsEqualTo(8);
        Check.That(mipMap.WrapMode).IsEqualTo(ImageWrap.Repeat);
        Check.That(MipMap.WeightLUTSize).IsEqualTo(128);
        Check.That(MipMap.WeightLut).CountIs(MipMap.WeightLUTSize);
        Check.That(mipMap.Levels).IsZero();
        
        var img = WriteImage(mipMap.ResampledImage, mipMap.Resolution);
        using var memoryStream = new MemoryStream();
        img.Save(memoryStream, ImageFormat.Png);
        var base64 = Convert.ToBase64String(memoryStream.ToArray());
        if (base64 == img_32x32)
        {
            Check.That(base64).IsEqualTo(img_32x32);
        }
        else
        {
            Check.That(base64).IsEqualTo(img_32x32_bis);
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
        else
        {
            Check.That(base64).IsEqualTo(img_32x32_clamp_bis);
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
        
}