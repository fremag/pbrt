using pbrt.Core;
using pbrt.Lights;
using pbrt.Shapes;
using pbrt.Spectrums;

namespace Pbrt.Demos.Scenes;

public class DragonGoldScene : DemoScene
{
    public DragonGoldScene()
    {
        var meshFactory = ReadModel("Pbrt.Demos.Models.dragon_mini.ply");

        Transform transform1 = RotateY(45);
        var scale = Scale(0.1f);
        transform1 = Translate(tY: 4f, tX: -2f) * transform1 * scale;
        var dragon1 = meshFactory.BuildTriangleMesh(transform1);
        AddPrimitives(dragon1, GetMetalMaterial("Au"));

        Floor();

        var lightToWorld = Translate(tY: 60) * RotateX(90);
        IShape shape = new Disk(lightToWorld, 10f);
        var light = new DiffuseAreaLight(lightToWorld, DefaultMediumInterface, new Spectrum(20f), 1, shape);
        AllLights.Add(light);
        
        PointLight(0, 3, -20, 300f);
    }
}