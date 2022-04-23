using System;
using System.Linq;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Shapes;
using Pbrt.Tests.Core;
using pbrt.Textures;

namespace Pbrt.Tests.Shapes
{
    [TestFixture]
    public class TriangleMeshTests
    {
        Transform t = Transform.Translate(-1,0,0);
        int[] vertexIndices = new int[]  {0, 1, 2};
        private Point3F[] point3Fs;
        private TriangleMesh mesh;
        private Triangle triangle;
        private Vector3F[] s;
        private Normal3F[] normals;
        private Point2F[] uvs;

        [SetUp]
        public void SetUp()
        {
            normals = null;
            s = null;
            uvs = null;
            point3Fs = new Point3F[]
            {
                new Point3F(0, 0, 0),
                new Point3F(2, 0, 0),
                new Point3F(1, 1, 0)
            };
            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, s, normals, uvs, null, null, null);
            triangle = mesh.GetTriangles().OfType<Triangle>().First();
        }
        
        [Test]
        public void BasicTest()
        {
            Check.That(triangle.Area).IsEqualTo(1f);
            Check.That(triangle.ObjectBound.PMin).IsEqualTo(new Point3F(0, 0, 0));
            Check.That(triangle.ObjectBound.PMax).IsEqualTo(new Point3F(2, 1, 0));
            Check.That(triangle.WorldBound().PMin).IsEqualTo(new Point3F(-1, 0, 0));
            Check.That(triangle.WorldBound().PMax).IsEqualTo(new Point3F(1, 1, 0));
            
            Point2F[] uv = new Point2F[3];
            triangle.GetUVs(uv);
            Point2F[] uvs = new []
            {
                new Point2F(0f, 0f),    
                new Point2F(1f, 0f),   
                new Point2F(1f, 1f)  
            };
            Check.That(uv).ContainsExactly(uvs);
        }

        [Test]
        public void GetUvTest()
        {
            Point2F[] uvs = new []
            {
                new Point2F(0.1f, 0.2f),    
                new Point2F(0.3f, 0.4f),   
                new Point2F(0.5f, 0.6f)  
            };
            
            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, null, null, uvs, null, null, null);
            triangle = mesh.GetTriangles().OfType<Triangle>().First();
            Point2F[] uv = new Point2F[3];
            triangle.GetUVs(uv);
            Check.That(uv).ContainsExactly(uvs);
        }

        [Test]
        public void Intersect_Test()
        {
            Point3F o = new Point3F(0, 0, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d);
            var result = triangle.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsTrue();
            Check.That(tHit).IsEqualTo(1);
            Check.That(isect.P).Check((0,0,0));
            Check.That(isect.N).Check((0,0,1));
        }

        [Test]
        public void Intersect_NoIntersection_T_vs_DeltaT_Test()
        {
            var x =  -0.5f;
            var y =  -0.1752f;
            var z =  1.4E-45f;
                
            Point3F o = new Point3F(-x, -y, -z);
            Vector3F d = new Vector3F(x, y, 1);
            Ray ray = new Ray(o, d);
            var result = triangle.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsFalse();
        }
        
        [Test]
        public void Intersect_NoIntersection_Test()
        {
            Point3F o = new Point3F(10f, 101f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d);
            var result = triangle.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(isect).IsNull();
        }

        [Test]
        public void Intersect_NoIntersection_Permutation_Test()
        {
           var point3Fs = new Point3F[]
            {
                new Point3F(1, 1, 0),
                new Point3F(1, 2, 0),
                new Point3F(2, 1, 0)
            }; 
            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, null, null, null, null, null, null);
            triangle = mesh.GetTriangles().OfType<Triangle>().First();

            Point3F o = new Point3F(0f, 0f, 0);
            Vector3F d = new Vector3F(0, 1, 0);
            Ray ray = new Ray(o, d);
            var result = triangle.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(isect).IsNull();
        }
        
        [Test]
        public void Intersect_NoIntersection_TMax_DetPositive_Test()
        {
            Point3F o = new Point3F(0f, 0f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) {TMax = 0.1f};
            var result = triangle.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(isect).IsNull();
        }
        
        [Test]
        public void Intersect_NoIntersection_TMax_DetNegative_Test()
        {
          var  point3Fs = new Point3F[]
            {
                new Point3F(2, 0, 0),
                new Point3F(0, 0, 0),
                new Point3F(1, 1, 0)
            }; 
            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, null, null, null, null, null, null);
            triangle = mesh.GetTriangles().OfType<Triangle>().First();

            Point3F o = new Point3F(0, 0.5f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) { TMax = 0.1f };
            var result = triangle.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(isect).IsNull();
        }

        [Test]
        public void Intersection_DegenerateUv_Test()
        {
           var point3Fs = new Point3F[]
            {
                new Point3F(2, 0, 0),
                new Point3F(0, 1e-30f, 0),
                new Point3F(1, 0, 0)
            }; 
            var uvs = new Point2F[] {Point2F.Zero, new Point2F(0, 0), new Point2F(0,0 ) };
            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, s, normals, uvs, null, null, null);
            triangle = mesh.GetTriangles().OfType<Triangle>().First();
 
            Point3F o = new Point3F(0f, 0f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) { TMax = 10f };
            var result = triangle.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(isect).IsNull();
        }

        [Test]
        public void Intersection_DegenerateUv_AlphaMaskZero_Test()
        {
            var point3Fs = new Point3F[]
            {
                new Point3F(2, 0, 0),
                new Point3F(0, 1, 0),
                new Point3F(1, 0, 0)
            }; 
            uvs = new Point2F[] {Point2F.Zero, new Point2F(0, 0), new Point2F(0,0 ) };
            Texture<float> alphaMask =new ConstantTexture<float>(0f);
            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, s, normals, uvs, alphaMask, null, null);
            triangle = mesh.GetTriangles().OfType<Triangle>().First();
 
            Point3F o = new Point3F(0f, 0f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) { TMax = 10f };
            var result = triangle.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(isect).IsNull();
        }
        
        [Test]
        public void Intersection_ReverseOrientation_Test()
        {
           var point3Fs = new Point3F[]
            {
                new Point3F(2, 0, 0),
                new Point3F(0, 1, 0),
                new Point3F(1, 0, 0)
            }; 
            uvs = new Point2F[] {Point2F.Zero, new Point2F(0, 0), new Point2F(0,0 ) };
            Texture<float> alphaMask =new ConstantTexture<float>(1f);
            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, s, normals, uvs, alphaMask, null, null);
            triangle = mesh.GetTriangles(true).OfType<Triangle>().First();
 
            Point3F o = new Point3F(0f, 0f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) { TMax = 10f };
            var result = triangle.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsTrue();
            Check.That(tHit).IsEqualTo(1);
            Check.That(isect.P).Check((0,0,0));
            Check.That(isect.N).Check((0,0,-1));
        }
        
        [Test]
        public void Intersection_ShadingNormals_Test()
        {
           var normals =  new Normal3F[]
            {
                new Normal3F(1, 0, 0), 
                new Normal3F(0, 1, 0), 
                new Normal3F(0, 0, 1), 
            }; 
            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, s, normals, uvs, null, null, null);
            triangle = mesh.GetTriangles(true).OfType<Triangle>().First();
 
            Point3F o = new Point3F(0f, 0f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) { TMax = 10f };
            var result = triangle.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsTrue();
            Check.That(tHit).IsEqualTo(1);
            Check.That(isect.P).Check((0,0,0));
            Check.That(isect.N).Check((0,0,-1));
            Check.That(isect.Shading.N.X).IsCloseTo(MathF.Sqrt(2)/2, 1e-6);
            Check.That(isect.Shading.N.Y).IsCloseTo(MathF.Sqrt(2)/2, 1e-6);
            Check.That(isect.Shading.N.Z).IsCloseTo(0f, 1e-6);
            Check.That(isect.Shading.DnDu.X).IsCloseTo(-1, 1e-6);
            Check.That(isect.Shading.DnDu.Y).IsCloseTo(1, 1e-6);
            Check.That(isect.Shading.DnDu.Z).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDv.X).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDv.Y).IsCloseTo(-1, 1e-6);
            Check.That(isect.Shading.DnDv.Z).IsCloseTo(1, 1e-6);
        }
        
        [Test]
        public void Intersection_ShadingNormals_LengthZero_Test()
        {
            var normals =  new Normal3F[]
            {
                new Normal3F(0, 0, 0), 
                new Normal3F(0, 0, 0), 
                new Normal3F(0, 0, 0), 
            }; 
            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, s, normals, uvs, null, null, null);
            triangle = mesh.GetTriangles(true).OfType<Triangle>().First();
 
            Point3F o = new Point3F(0f, 0f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) { TMax = 10f };
            var result = triangle.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsTrue();
            Check.That(tHit).IsEqualTo(1);
            Check.That(isect.P).Check((0,0,0));
            Check.That(isect.N).Check((0,0,-1));
            Check.That(isect.Shading.N.X).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.N.Y).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.N.Z).IsCloseTo(-1, 1e-6);
            Check.That(isect.Shading.DnDu.X).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDu.Y).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDu.Z).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDv.X).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDv.Y).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDv.Z).IsCloseTo(0, 1e-6);
        }
        
        [Test]
        public void Intersection_ShadingNormals_DegenerateUvs_Test()
        {
            var normals =  new Normal3F[]
            {
                new Normal3F(1, 0, 0), 
                new Normal3F(0, 1, 0), 
                new Normal3F(0, 0, 1), 
            };
            
            Point2F[] uvs = new []
            {
                new Point2F(1e-12f, 0.0f),    
                new Point2F(1e-13f, 0.0f),   
                new Point2F(1e-14f, 0.0f)  
            };

            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, s, normals, uvs, null, null, null);
            triangle = mesh.GetTriangles(true).OfType<Triangle>().First();
 
            Point3F o = new Point3F(0f, 0f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) { TMax = 10f };
            var result = triangle.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsTrue();
            Check.That(tHit).IsEqualTo(1);
            Check.That(isect.P).Check((0,0,0));
            Check.That(isect.N).Check((0,0,-1));
            Check.That(isect.Shading.N.X).IsCloseTo(MathF.Sqrt(2)/2, 1e-6);
            Check.That(isect.Shading.N.Y).IsCloseTo(MathF.Sqrt(2)/2, 1e-6);
            Check.That(isect.Shading.N.Z).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDu.X).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDu.Y).IsCloseTo(-MathF.Sqrt(2)/2, 1e-6);
            Check.That(isect.Shading.DnDu.Z).IsCloseTo(MathF.Sqrt(2)/2, 1e-6);
            Check.That(isect.Shading.DnDv.X).IsCloseTo(-MathF.Sqrt(2), 1e-6);
            Check.That(isect.Shading.DnDv.Y).IsCloseTo(MathF.Sqrt(2)/2, 1e-6);
            Check.That(isect.Shading.DnDv.Z).IsCloseTo(MathF.Sqrt(2)/2, 1e-6);
        }
        
        [Test]
        public void Intersection_ShadingNormals_DegenerateUvs_LengthZero_Test()
        {
            var normals =  new Normal3F[]
            {
                new Normal3F(0, 0, 0), 
                new Normal3F(0, 0, 0), 
                new Normal3F(0, 0, 0), 
            };
            
            Point2F[] uvs = new []
            {
                new Point2F(1e-12f, 0.0f),    
                new Point2F(1e-13f, 0.0f),   
                new Point2F(1e-14f, 0.0f)  
            };

            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, s, normals, uvs, null, null, null);
            triangle = mesh.GetTriangles(true).OfType<Triangle>().First();
 
            Point3F o = new Point3F(0f, 0f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) { TMax = 10f };
            var result = triangle.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsTrue();
            Check.That(tHit).IsEqualTo(1);
            Check.That(isect.P).Check((0,0,0));
            Check.That(isect.N).Check((0,0,-1));
            Check.That(isect.Shading.N.X).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.N.Y).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.N.Z).IsCloseTo(-1, 1e-6);
            Check.That(isect.Shading.DnDu.X).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDu.Y).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDu.Z).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDv.X).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDv.Y).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDv.Z).IsCloseTo(0, 1e-6);
        }
        
        [Test]
        public void Intersection_ShadingSurface_Test()
        {
            var s =  new Vector3F[]
            {
                new Vector3F(1, 0, 0), 
                new Vector3F(0, 1, 0), 
                new Vector3F(0, 0, 1), 
            }; 
            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, s, normals, uvs, null, null, null);
            triangle = mesh.GetTriangles(true).OfType<Triangle>().First();
 
            Point3F o = new Point3F(0f, 0f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) { TMax = 10f };
            var result = triangle.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsTrue();
            Check.That(tHit).IsEqualTo(1);
            Check.That(isect.P).Check((0,0,0));
            Check.That(isect.N).Check((0,0,-1));
            Check.That(isect.Shading.N.X).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.N.Y).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.N.Z).IsCloseTo(-1f, 1e-6);
            Check.That(isect.Shading.DnDu.X).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDu.Y).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDu.Z).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDv.X).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDv.Y).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDv.Z).IsCloseTo(0, 1e-6);
        }
        
        [Test]
        public void Intersection_ShadingSurface_LengthZero_Test()
        {
            var s =  new Vector3F[]
            {
                new Vector3F(0, 0, 0), 
                new Vector3F(0, 0, 0), 
                new Vector3F(0, 0, 0), 
            }; 
            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, s, normals, uvs, null, null, null);
            triangle = mesh.GetTriangles(true).OfType<Triangle>().First();
 
            Point3F o = new Point3F(0f, 0f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) { TMax = 10f };
            var result = triangle.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsTrue();
            Check.That(tHit).IsEqualTo(1);
            Check.That(isect.P).Check((0,0,0));
            Check.That(isect.N).Check((0,0,-1));
            Check.That(isect.Shading.N.X).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.N.Y).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.N.Z).IsCloseTo(-1f, 1e-6);
            Check.That(isect.Shading.DnDu.X).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDu.Y).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDu.Z).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDv.X).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDv.Y).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDv.Z).IsCloseTo(0, 1e-6);
        }
         
        [Test]
        public void Intersection_Shading_Bitangent_Test()
        {
            var s =  new Vector3F[]
            {
                new Vector3F(1, 0, 0), 
                new Vector3F(0, 1, 0), 
                new Vector3F(0, 0, 1), 
            }; 

            var normals =  new Normal3F[]
            {
                new Normal3F(0, 1, 0), 
                new Normal3F(1, 0, 0), 
                new Normal3F(0, 0, 1), 
            };

            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, s, normals, uvs, null, null, null);
            triangle = mesh.GetTriangles(true).OfType<Triangle>().First();
 
            Point3F o = new Point3F(0f, 0f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) { TMax = 10f };
            var result = triangle.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsTrue();
            Check.That(tHit).IsEqualTo(1);
            Check.That(isect.P).Check((0,0,0));
            Check.That(isect.N).Check((0,0,-1));
            Check.That(isect.Shading.N.X).IsCloseTo(MathF.Sqrt(2)/2, 1e-6);
            Check.That(isect.Shading.N.Y).IsCloseTo(MathF.Sqrt(2)/2, 1e-6);
            Check.That(isect.Shading.N.Z).IsCloseTo(0f, 1e-6);
            Check.That(isect.Shading.DnDu.X).IsCloseTo(1, 1e-6);
            Check.That(isect.Shading.DnDu.Y).IsCloseTo(-1, 1e-6);
            Check.That(isect.Shading.DnDu.Z).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDv.X).IsCloseTo(-1, 1e-6);
            Check.That(isect.Shading.DnDv.Y).IsCloseTo(0, 1e-6);
            Check.That(isect.Shading.DnDv.Z).IsCloseTo(1, 1e-6);
        }
 
        
        [Test]
        public void IntersectP_Test()
        {
            Point3F o = new Point3F(0, 0, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d);
            var result = triangle.IntersectP(ray);
            Check.That(result).IsTrue();
        }

        [Test]
        public void IntersectP_NoIntersection_T_vs_DeltaT_Test()
        {
            var x =  -0.5f;
            var y =  -0.1752f;
            var z =  1.4E-45f;
                
            Point3F o = new Point3F(-x, -y, -z);
            Vector3F d = new Vector3F(x, y, 1);
            Ray ray = new Ray(o, d);
            var result = triangle.IntersectP(ray);
            Check.That(result).IsFalse();
        }
        
        [Test]
        public void IntersectP_NoIntersection_Test()
        {
            Point3F o = new Point3F(10f, 101f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d);
            var result = triangle.IntersectP(ray);
            Check.That(result).IsFalse();
        }

        [Test]
        public void IntersectP_NoIntersection_Permutation_Test()
        {
           var point3Fs = new Point3F[]
            {
                new Point3F(1, 1, 0),
                new Point3F(1, 2, 0),
                new Point3F(2, 1, 0)
            }; 
            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, null, null, null, null, null, null);
            triangle = mesh.GetTriangles().OfType<Triangle>().First();

            Point3F o = new Point3F(0f, 0f, 0);
            Vector3F d = new Vector3F(0, 1, 0);
            Ray ray = new Ray(o, d);
            var result = triangle.IntersectP(ray);
            Check.That(result).IsFalse();
        }
        
        [Test]
        public void IntersectP_NoIntersection_TMax_DetPositive_Test()
        {
            Point3F o = new Point3F(0f, 0f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) {TMax = 0.1f};
            var result = triangle.IntersectP(ray);
            Check.That(result).IsFalse();
        }
        
        [Test]
        public void IntersectP_NoIntersection_TMax_DetNegative_Test()
        {
          var  point3Fs = new Point3F[]
            {
                new Point3F(2, 0, 0),
                new Point3F(0, 0, 0),
                new Point3F(1, 1, 0)
            }; 
            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, null, null, null, null, null, null);
            triangle = mesh.GetTriangles().OfType<Triangle>().First();

            Point3F o = new Point3F(0, 0.5f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) { TMax = 0.1f };
            var result = triangle.IntersectP(ray);
            Check.That(result).IsFalse();
        }

        [Test]
        public void IntersectP_DegenerateUv_Test()
        {
           var point3Fs = new Point3F[]
            {
                new Point3F(2, 0, 0),
                new Point3F(0, 1e-30f, 0),
                new Point3F(1, 0, 0)
            }; 
            var uvs = new Point2F[] {Point2F.Zero, new Point2F(0, 0), new Point2F(0,0 ) };
            Texture<float> alphaMask = new ConstantTexture<float>(1f);
            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, s, normals, uvs, alphaMask, null, null);
            triangle = mesh.GetTriangles().OfType<Triangle>().First();
 
            Point3F o = new Point3F(0f, 0f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) { TMax = 10f };
            var result = triangle.IntersectP(ray);
            Check.That(result).IsFalse();
        }

        [Test]
        public void IntersectP_NotDegenerateUv_Test()
        {
           var point3Fs = new Point3F[]
            {
                new Point3F(2, 0, 0),
                new Point3F(0, 1f, 0),
                new Point3F(1, 0, 0)
            }; 
            var uvs = new Point2F[] {Point2F.Zero, new Point2F(1, 0), new Point2F(0, 1 ) };
            Texture<float> alphaMask = new ConstantTexture<float>(1f);
            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, s, normals, uvs, alphaMask, null, null);
            triangle = mesh.GetTriangles().OfType<Triangle>().First();
 
            Point3F o = new Point3F(0f, 0f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) { TMax = 10f };
            var result = triangle.IntersectP(ray);
            Check.That(result).IsTrue();
        }

        [Test]
        public void IntersectP_NotDegenerateUv_ShadowMask_Test()
        {
           var point3Fs = new Point3F[]
            {
                new Point3F(2, 0, 0),
                new Point3F(0, 1f, 0),
                new Point3F(1, 0, 0)
            }; 
            var uvs = new Point2F[] {Point2F.Zero, new Point2F(1, 0), new Point2F(0, 1 ) };
            Texture<float> alphaMask = new ConstantTexture<float>(1f);
            Texture<float> shadowMask = new ConstantTexture<float>(0f);
            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, s, normals, uvs, alphaMask, shadowMask, null);
            triangle = mesh.GetTriangles().OfType<Triangle>().First();
 
            Point3F o = new Point3F(0f, 0f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) { TMax = 10f };
            var result = triangle.IntersectP(ray);
            Check.That(result).IsFalse();
        }

        [Test]
        public void IntersectP_DegenerateUv_AlphaMaskZero_Test()
        {
            var point3Fs = new Point3F[]
            {
                new Point3F(2, 0, 0),
                new Point3F(0, 1, 0),
                new Point3F(1, 0, 0)
            }; 
            uvs = new Point2F[] {Point2F.Zero, new Point2F(0, 0), new Point2F(0,0 ) };
            Texture<float> alphaMask =new ConstantTexture<float>(0f);
            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, s, normals, uvs, alphaMask, null, null);
            triangle = mesh.GetTriangles().OfType<Triangle>().First();
 
            Point3F o = new Point3F(0f, 0f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) { TMax = 10f };
            var result = triangle.IntersectP(ray);
            Check.That(result).IsFalse();
        }
        
        [Test]
        public void IntersectionP_ReverseOrientation_Test()
        {
           var point3Fs = new Point3F[]
            {
                new Point3F(2, 0, 0),
                new Point3F(0, 1, 0),
                new Point3F(1, 0, 0)
            }; 
            uvs = new Point2F[] {Point2F.Zero, new Point2F(0, 0), new Point2F(0,0 ) };
            Texture<float> alphaMask =new ConstantTexture<float>(1f);
            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, s, normals, uvs, alphaMask, null, null);
            triangle = mesh.GetTriangles(true).OfType<Triangle>().First();
 
            Point3F o = new Point3F(0f, 0f, -1);
            Vector3F d = new Vector3F(0, 0, 1);
            Ray ray = new Ray(o, d) { TMax = 10f };
            var result = triangle.IntersectP(ray);
            Check.That(result).IsTrue();
        }

        [Test]
        public void SampleTest()
        {
            triangle.ReverseOrientation = true;
            var interaction = triangle.Sample(new Point2F(0, 0));
            Check.That(interaction.P).IsEqualTo(new Point3F(-1,0,0));
            Check.That(interaction.N).IsEqualTo(new Normal3F(0,0,-1));

            triangle.ReverseOrientation = false;
            interaction = triangle.Sample(new Point2F(0, 0));
            Check.That(interaction.P).IsEqualTo(new Point3F(-1,0,0));
            Check.That(interaction.N).IsEqualTo(new Normal3F(0,0,1));
        }

        [Test]
        public void Sample_NormalsTest()
        {
 
            var normals =  new Normal3F[]
            {
                new Normal3F(0, 1, 0), 
                new Normal3F(1, 0, 0), 
                new Normal3F(0, 0, 2), 
            };

            mesh = new TriangleMesh(t, 1, vertexIndices, vertexIndices.Length, point3Fs, s, normals, uvs, null, null, null);
            triangle = mesh.GetTriangles(true).OfType<Triangle>().First();
            
            var interaction = triangle.Sample(new Point2F(0, 0));
            Check.That(interaction.P).IsEqualTo(new Point3F(-1,0,0));
            Check.That(interaction.N).IsEqualTo(new Normal3F(0,0,1));
        }

        [Test]
        public void TriangleMesh_Test()
        {
            int[] vertexIndices = {0, 1, 2, 0, 1, 3};
            var point3Fs = new Point3F[]
            {
                new Point3F(0, 0, 0),
                new Point3F(2, 0, 0),
                new Point3F(1, 1, 0),
                new Point3F(1, -1, 0)
            };

            int[] fIndices = Array.Empty<int>();
            var mesh = new TriangleMesh(t, 2, vertexIndices, vertexIndices.Length, point3Fs, s, normals, uvs, null, null, fIndices);
            Check.That(mesh.NbVertices).IsEqualTo(vertexIndices.Length);
            Check.That(mesh.VertexIndices).ContainsExactly(vertexIndices);
            Check.That(mesh.FaceIndices).IsEmpty();

            var shapes = mesh.GetTriangles();
            var triangles = shapes.OfType<Triangle>().ToArray();
            Check.That(triangles).CountIs(2);
            Check.That(triangles[0].P0).IsEqualTo(new Point3F(-1,0,0));
            Check.That(triangles[0].P1).IsEqualTo(new Point3F(1,0,0));
            Check.That(triangles[0].P2).IsEqualTo(new Point3F(0,1,0));
            Check.That(triangles[1].P0).IsEqualTo(new Point3F(-1,0,0));
            Check.That(triangles[1].P1).IsEqualTo(new Point3F(1,0,0));
            Check.That(triangles[1].P2).IsEqualTo(new Point3F(0,-1,0));
        }
    }
}