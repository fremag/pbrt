using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using pbrt.Accelerators;
using pbrt.Core;
using Pbrt.Demos.Ply;
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

        public static MediumInterface DefaultMediumInterface { get; } = new MediumInterface(HomogeneousMedium.Default());

        public Texture<float> MakeTexture(float f) => new ConstantTexture<float>(f);
        public Texture<Spectrum> MakeSpectrumTexture(float[] rgb) => MakeSpectrumTexture(rgb[0], rgb[1], rgb[2]);
        public Texture<Spectrum> MakeSpectrumTexture(float f) => new ConstantTexture<Spectrum>(new Spectrum(f));
        public Texture<Spectrum> MakeSpectrumTexture(float r, float g, float b) => new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new[] { r, g, b })));
        public Texture<Spectrum> MakeGraySpectrumTexture(float level) => new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new[] { level, level, level })));

        public List<IPrimitive> AllPrimitives { get; private set; } = new List<IPrimitive>();
        public List<Light> AllLights { get; private set; } = new List<Light>();

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
        public Transform Scale(float s = 1) => Transform.Scale(s, s, s);
        public Transform Scale(float sX = 1, float sY = 1, float sZ = 1) => Transform.Scale(sX, sY, sZ);
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

        public TriangleMesh Plane(float x0, float z0, float x1, float z1)
        {
            int[] indices = {0, 1, 2, 0, 3, 2};
            Point3F[] points = {
                new(x0, 0, z0), 
                new(x0, 0, z1 ), 
                new(x1, 0, z1), 
                new(x1, 0, z0) 
            };
            
            var plane = new TriangleMesh(Translate(), 2, indices, 4, points, null, null, null, null, null, null);
            return plane;
        }
        
        public void Floor()
        {
            Texture<Spectrum> kdWhite = MakeGraySpectrumTexture(1);
            Texture<Spectrum> kdGray = MakeGraySpectrumTexture(0.5f);
            TextureMapping3D mapping = new TextureMapping3D(Translate(0));
            var kdChecker = new Checkerboard3DTexture<Spectrum>(mapping, kdGray, kdWhite);
            var matteMaterial = new MatteMaterial(kdChecker, MakeTexture(50f), null);

            var plane = Plane(-1000, -1000, 1000, 1000);
            AddPrimitives(BuildTriangles(plane, matteMaterial));
        }

        public TriangleMesh Cube(Transform transform)
        {
            return Cube(-1, -1, -1, 1, 1, 1, transform);
        }

        public TriangleMesh Cube(float x0, float y0, float z0, float x1, float y1, float z1)
        {
            return Cube(x0, y0, z0, x1, y1, z1, Translate());
        }
        
        public TriangleMesh Cube(float x0, float y0, float z0, float x1, float y1, float z1, Transform transform)
        {
            int[] indices = {
                0, 2, 3, 
                0, 1, 3,
                0, 2, 6,
                0, 4, 6,
                0, 1, 5, 
                0, 4, 5,
                7, 2, 3,
                7, 2, 6,
                7, 4, 6,
                7, 4, 5,
                7, 1, 3,
                7, 1, 5
            };
            
            Point3F[] points = {
                new(x0, y0, z0), 
                new(x0, y0, z1), 
                new(x0, y1, z0), 
                new(x0, y1, z1),
                
                new(x1, y0, z0), 
                new(x1, y0, z1), 
                new(x1, y1, z0), 
                new(x1, y1, z1), 
            };
            
            var cube = new TriangleMesh(transform, 12, indices, 4, points, null, null, null, null, null, null);
            return cube;
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

        protected void AddPrimitives(TriangleMesh primitives, IMaterial material)
        {
            var triangles = BuildTriangles(primitives, material);
            AddPrimitives(triangles);
        }

        public void Axis()
        {
            var cube1 = Cube(-0.1f, 0f, -0.1f, 0.1f, 20f, 0.1f);
            var cube2 = Cube(-0.1f, -0.1f, 0f, 0.1f, 0.1f, 20f);
            var cube3 = Cube(0f, -0.1f, -0.1f, 20f, 0.1f, 0.1f);
            
            AddPrimitives(BuildTriangles(cube1, PlasticMaterialGreen));
            AddPrimitives(BuildTriangles(cube2, PlasticMaterialRed));
            AddPrimitives(BuildTriangles(cube3, PlasticMaterialBlue));
        }

        protected float[] Rgb(int r, int g, int b) => new float[] { r / 255f, g / 255f, b / 255f };

        public MeshFactory ReadModel(string name)
        {
            var assembly = Assembly.GetAssembly(typeof(Dragon2Scene));
            using Stream stream = assembly.GetManifestResourceStream(name);

            var meshFactory = new MeshFactory(stream);
            return meshFactory;
        }
    }
}