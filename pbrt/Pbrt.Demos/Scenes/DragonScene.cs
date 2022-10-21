using Pbrt.Demos.Ply;

namespace Pbrt.Demos.Scenes;

public class DragonScene : DemoScene
{
    public DragonScene()
    {
        var path = @"E:\Projects\pbrt-scenes\pbrt-v3-scenes\dragon\geometry\dragon_remeshed.ply";
        MeshFactory meshFactory = new MeshFactory(path);
        
        var transform1 =  (RotateY(45)*RotateZ(90)*RotateY(45));
        var scale = Scale(0.05f);
        transform1 = Translate(tY: 2.2f, tX: -3f) * transform1 * scale;
        var dragon1 = meshFactory.BuildTriangleMesh(transform1);
        
        var transform2 =  (RotateY(-15)*RotateZ(90)*RotateY(45));
        transform2 = Translate(tY: 2.2f, tX: 5f) * transform2 * scale;
        var dragon2 =  meshFactory.BuildTriangleMesh(transform2);;

        Floor();
        AddPrimitives(dragon1, PlasticMaterialGreen);
        AddPrimitives(dragon2, MakeMatteMaterial(30, 0.7f, 0.7f, 1f));
        
        PointLight( 0, 1, -20, 500f);
        PointLight(0, 50, 0, 2500f);
    }
}