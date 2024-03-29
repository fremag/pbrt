using pbrt.Core;

namespace Pbrt.Demos.Scenes;

public class Dragon2Scene : DemoScene
{
    public Dragon2Scene()
    {
        var meshFactory = ReadModel("Pbrt.Demos.Models.dragon_mini.ply");

        Transform transform1 = RotateY(45);
        var scale = Scale(0.1f);
        transform1 = Translate(tY: 4f, tX: -2f) * transform1 * scale;
        var dragon1 = meshFactory.BuildTriangleMesh(transform1);

        Floor();
        AddPrimitives(dragon1, PlasticMaterialGreen);
        PointLight(0, 3, -20, 300f);
        PointLight(0, 12, 0, 300f);
    }
}