using System;
using System.IO;
using NFluent;
using NSubstitute;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Lights;
using pbrt.Shapes;
using pbrt.Spectrums;
using pbrt.Textures;

namespace Pbrt.Tests.Lights;

[TestFixture]
public class InfiniteLightTests
{
    private InfiniteAreaLight light;
    private const string Img20X20 = "iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAIAAAAC64paAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAB/SURBVDhP5Y1RDsAgCEO9/6UdCiqU4va/lxhpabV1RkvYIoJuFaV+0LTmgcARr03Fx2y6N2UbOmsel18Ilcz9ebKbHD8IO3YshTqVie5H/lrWa2sdKiAZykLVFz/HTMPiIoUtQ8imyawMTC+8gwWbCiBAHqZPUJ9/pVGPLTy9P1bDyGI4SbwfAAAAAElFTkSuQmCC";
    private Scene scene;
    
    [SetUp]
    public void SetUp()
    {
        byte[] imgBytes = Convert.FromBase64String(Img20X20);
        using var stream = new MemoryStream(imgBytes, 0, imgBytes.Length);

        var l = new Spectrum(2f);
        Transform transform = Transform.Translate(0f, 0f, 0f);
        light = new InfiniteAreaLight("img", stream, l, transform, 1);

        IShape shape = Substitute.For<IShape>();
        shape.WorldBound().Returns(new Bounds3F(new Point3F(-2,-2,-2), new Point3F(1,1,1)));
        IPrimitive aggregate = new GeometricPrimitive(shape, null, null, null);
        scene = new Scene(aggregate, new Light[] {light});
        scene.Init();
    }

    [TearDown]
    public void Teardown()
    {
        MipMap.ClearCache();        
    }
    
    [Test]
    public void PowerTest()
    {
        var power = light.Power();
        var rgb = power.ToRgb();
        Check.That(rgb[0]).IsCloseTo(31.6716805f, 1e-5f);
        Check.That(rgb[1]).IsCloseTo(31.5899811f, 1e-5f);
        Check.That(rgb[2]).IsCloseTo(31.3259811f, 1e-5f);
    }

    [Test]
    public void BasicTest()
    {
        Check.That(light.WorldCenter).IsEqualTo(new Point3F(-0.5f, -0.5f, -0.5f));
        Check.That(light.WorldRadius).IsEqualTo(2.598076f);
        Check.That(light.Distribution).Not.IsNull();
    }

    [Test]
    public void LeTest()
    {
        Spectrum le = light.Le(new RayDifferential((0, 0, -1), (0, 0, 1)));
        var rgb = le.ToRgb();
        Check.That(rgb).ContainsExactly(0.727240562f, 0.725364506f, 0.719302118f);
    }

    [Test]
    public void Pdf_LiTest()
    {
        Vector3F wi = (0, 0, 1);
        Interaction interaction = new Interaction(Point3F.Zero, 1.23f, null);
        var pdfLi = light.Pdf_Li(interaction, wi);
        Check.That(pdfLi).IsZero();
        
        wi = (1, 1, 0);
        pdfLi = light.Pdf_Li(interaction, wi);
        Check.That(pdfLi).IsCloseTo(0.0864505768f, 1e-5f);
    }

    [Test]
    public void SampleLiTest()
    {
        Interaction interaction = new Interaction(Point3F.Zero, 1.23f, null);
        Point2F u = new (0,0);
        var li = light.Sample_Li(interaction, u, out var wi, out var pdf, out var visibilityTester);
        Check.That(pdf).IsEqualTo(0);
        var rgb = li.ToRgb();
        Check.That(rgb[0]).IsCloseTo(1.45448112f, 1e-5f);
        Check.That(rgb[1]).IsCloseTo(1.45072901f, 1e-5f);
        Check.That(rgb[2]).IsCloseTo(1.438604240f, 1e-5f);
    }
}