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
        transform1 = Translate(tY: 4f, tX: -2.5f) * transform1 * scale;
        var dragon1 = meshFactory.BuildTriangleMesh(transform1);
        AddPrimitives(dragon1, GetMetalMaterial("Au"));

        Floor();
        
        var lightPath = "Pbrt.Demos.Lights.skylight-day.png";
        var stream = GetResource(lightPath); 
        var infLight = new InfiniteAreaLight(lightPath, stream, new Spectrum(4), RotateY(0)* RotateX(-90));
        AllLights.Add(infLight);

        var lightToWorld = Translate(tY: 2, tZ: -10) * RotateY(0);
        IShape shape = new Disk(lightToWorld, 4f);
        var light = new DiffuseAreaLight(lightToWorld, DefaultMediumInterface, new Spectrum(7f), 1, shape);
        AllLights.Add(light);
    }
}