using System;
using System.IO;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Lights;
using pbrt.Spectrums;
using pbrt.Textures;

namespace Pbrt.Tests.Lights;

[TestFixture]
public class InfiniteLightTests
{
    private InfiniteAreaLight light;
    private const string Img20X20 = "iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAIAAAAC64paAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAB/SURBVDhP5Y1RDsAgCEO9/6UdCiqU4va/lxhpabV1RkvYIoJuFaV+0LTmgcARr03Fx2y6N2UbOmsel18Ilcz9ebKbHD8IO3YshTqVie5H/lrWa2sdKiAZykLVFz/HTMPiIoUtQ8imyawMTC+8gwWbCiBAHqZPUJ9/pVGPLTy9P1bDyGI4SbwfAAAAAElFTkSuQmCC";

    [SetUp]
    public void SetUp()
    {
        byte[] imgBytes = Convert.FromBase64String(Img20X20);
        using var stream = new MemoryStream(imgBytes, 0, imgBytes.Length);

        var l = new Spectrum(2f);
        Transform transform = Transform.Translate(0f, 0f, 0f);
        light = new InfiniteAreaLight("img", stream, l, transform, 1);
    }

    [TearDown]
    public void Teardown()
    {
        MipMap.ClearCache();        
    }
    
    [Test]
    public void BasicTest()
    {
        
    }
}