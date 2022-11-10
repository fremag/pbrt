using pbrt.Lights;
using pbrt.Spectrums;

namespace Pbrt.Demos.Scenes;

public class InfinityLightScene : DemoScene
{
    public InfinityLightScene(string lightName, float rotYDeg)
    {
        Sphere(0, 1.5f, 0, 1.5f, MatteMaterialGreen());
        Floor();
        var lightPath = $"Pbrt.Demos.Lights.{lightName}.png";
        var stream = GetResource(lightPath); 
        InfiniteAreaLight light = new InfiniteAreaLight(lightPath, stream, new Spectrum(1000), RotateY(rotYDeg)* RotateX(-90));
        AllLights.Add(light);
    }
}