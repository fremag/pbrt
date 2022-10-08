using System;
using System.Collections.Generic;
using System.Linq;
using pbrt.Accelerators;
using pbrt.Core;
using pbrt.Lights;
using pbrt.Materials;
using pbrt.Media;
using pbrt.Shapes;
using pbrt.Spectrums;
using pbrt.Textures;

namespace Pbrt.Demos.Scenes
{
    public class DemoScene : Scene
    {
        public static Vector3F VX => new Vector3F(1, 0, 0);
        public static Vector3F VY => new Vector3F(0, 1, 0);
        public static Vector3F VZ => new Vector3F(0, 0, 1);

        public IMaterial MatteMaterialRed(float sigma = 50, float bump = 0) => MakeMatteMaterial(sigma, 1, 0, 0, bump);
        public IMaterial MatteMaterialGreen(float sigma = 50, float bump = 0) => MakeMatteMaterial(sigma, 0, 1, 0, bump);
        public IMaterial MatteMaterialBlue(float sigma = 50, float bump = 0) => MakeMatteMaterial(sigma, 0, 0, 1, bump);

        public static MediumInterface DefaultMediumInterface => new MediumInterface(HomogeneousMedium.Default());

        public Texture<float> MakeTexture(float f) => new ConstantTexture<float>(f);
        public Texture<Spectrum> MakeSpectrumTexture(float f) => new ConstantTexture<Spectrum>(new Spectrum(f));
        public Texture<Spectrum> MakeSpectrumTexture(float r, float g, float b) => new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new[] { r, g, b })));
        public Texture<Spectrum> MakeGraySpectrumTexture(float level) => new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new[] { level, level, level })));

        public readonly List<IPrimitive> AllPrimitives = new List<IPrimitive>();
        public readonly List<Light> AllLights = new List<Light>();

        public IMaterial MakeMatteMaterial(float sigma, float r, float g, float b, float bump = 0)
        {
            Texture<Spectrum> kd = MakeSpectrumTexture(r, g, b);
            Texture<float> sigmaTexture = MakeTexture(sigma);

            Texture<float> bumpMap = bump != 0 ? MakeTexture(bump) : null;
            return new MatteMaterial(kd, sigmaTexture, bumpMap);
        }

        public IMaterial PlasticMaterialRed => PlasticMaterial(50f, 1, 0, 0, 0.001f);
        public IMaterial PlasticMaterialGreen => PlasticMaterial(50f, 0, 1, 0, 0.001f);
        public IMaterial PlasticMaterialBlue => PlasticMaterial(50f, 0, 0, 1, 0.001f);

        public IMaterial PlasticMaterial(float ks, float r, float g, float b, float roughness, float bumpMap = 0)
        {
            Texture<float> bumpMapTexture = bumpMap == 0 ? null : MakeTexture(bumpMap);
            var plasticMaterial = new PlasticMaterial(MakeSpectrumTexture(r, g, b), MakeSpectrumTexture(ks), MakeTexture(roughness), bumpMapTexture, false);
            return plasticMaterial;
        }

        public Transform Translate(float tX = 0, float tY = 0, float tZ = 0) => Transform.Translate(tX, tY, tZ);
        public Transform Scale(float sX = 0, float sY = 0, float sZ = 0) => Transform.Scale(sX, sY, sZ);
        public Transform RotateX(float deg) => Transform.RotateX(deg);
        public Transform RotateY(float deg) => Transform.RotateY(deg);
        public Transform RotateZ(float deg) => Transform.RotateZ(deg);

        public GeometricPrimitive Sphere(float x, float y, float z, float radius, IMaterial material, AreaLight areaLight = null, MediumInterface mediumInterface = null)
        {
            Transform objectToWorld = Translate(x, y, z);
            var sphere = new Sphere(objectToWorld, radius);

            var primitiveMediumInterface = mediumInterface ?? DefaultMediumInterface;
            return AddShape(sphere, material, areaLight, primitiveMediumInterface);
        }

        public override void Init()
        {
            if (!AllPrimitives.Any())
            {
                throw new InvalidOperationException("no primitives in scene !");
            }

            if (!AllLights.Any())
            {
                throw new InvalidOperationException("no lights in scene !");
            }

            var bvhAccel = new BvhAccel(AllPrimitives, 5, SplitMethod.Middle);
            base.Init(bvhAccel, AllLights.ToArray());
        }

        public PointLight PointLight(float x, float y, float z, float spectrumValue)
        {
            var pointLight = new PointLight(Translate(x, y, z), DefaultMediumInterface, new Spectrum(spectrumValue));
            AllLights.Add(pointLight);
            return pointLight;
        }

        public void Floor()
        {
            var planeTransform = Transform.Scale(100f, 0.1f, 100f);
            var plane = new Sphere(planeTransform, 1f);
            Texture<Spectrum> kdWhite = MakeGraySpectrumTexture(1);
            Texture<Spectrum> kdGray = MakeGraySpectrumTexture(0.5f);
            TextureMapping2D mapping = new PlanarMapping2D(VX, VZ, 1, 1);
            var kdChecker = new Checkerboard2DTexture<Spectrum>(mapping, kdGray, kdWhite);
            AddShape(plane, new MatteMaterial(kdChecker, MakeTexture(50f), null));
        }

        public void Cylinder(Transform transform, IMaterial material, float zMax = 1f, MediumInterface mediumInterface = null)
        {
            var cylinder = new Cylinder(transform, zMax: zMax);
            AddShape(cylinder, material, null, mediumInterface);
        }

        public GeometricPrimitive AddShape(IShape shape, IMaterial material, AreaLight areaLight = null, MediumInterface mediumInterface = null)
        {
            var primitiveMediumInterface = mediumInterface ?? DefaultMediumInterface;
            var primitive = new GeometricPrimitive(shape, material, areaLight, primitiveMediumInterface);
            AllPrimitives.Add(primitive);
            return primitive;
        }

        public void Disk(Transform transform, IMaterial material, float radius = 0.5f, MediumInterface mediumInterface = null)
        {
            var disk = new Disk(transform, radius);
            AddShape(disk, material, null, mediumInterface);
        }

        public void AddPrimitives(IEnumerable<IPrimitive> primitives)
        {
            AllPrimitives.AddRange(primitives);
        }


        public IEnumerable<IPrimitive> BuildTriangles(TriangleMesh mesh, IMaterial material)
        {
            var triangles = mesh
                .GetTriangles()
                .Select(shape => new GeometricPrimitive(shape, material, null, DefaultMediumInterface))
                .ToArray();
            return triangles;
        }
    }
}