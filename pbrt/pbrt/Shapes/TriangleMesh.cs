using System;
using System.Collections.Generic;
using pbrt.Core;
using pbrt.Textures;

namespace pbrt.Shapes
{
    public class TriangleMesh
    {
        public Transform ObjectToWorld { get; }
        public Transform WorldToObject { get; }
        public int NbTriangles { get; }
        public int NbVertices { get; }
        public List<int> VertexIndices{ get; }
        public Point3F[] P{ get; }
        public Normal3F[] N{ get; }
        public Vector3F[] S{ get;  }
        public Point2F[] Uv{ get; }
        public Texture<float> AlphaMask{ get; }
        public Texture<float> ShadowAlphaMask{ get; }
        public List<int> FaceIndices{ get; }
        
        public TriangleMesh(Transform objectToWorld, int nbTriangles, int[] vertexIndices,
        int nbVertices, Point3F[] p, Vector3F[] s, Normal3F[] n,
        Point2F[] uv, Texture<float> alphaMask, Texture<float> shadowAlphaMask, int[] fIndices)
        {
            ObjectToWorld = objectToWorld;
            WorldToObject = objectToWorld.Inverse();
            NbTriangles = nbTriangles;
            NbVertices = nbVertices;
            VertexIndices = new List<int>(vertexIndices);
            AlphaMask = alphaMask;
            ShadowAlphaMask = shadowAlphaMask;
            
            P = new Point3F[nbTriangles*3];
            // Transform mesh vertices to world space
            for (int i = 0; i < nbVertices; ++i)
            {
                var vertexIndex = vertexIndices[i];
                var point = p[vertexIndex];
                P[i] = objectToWorld.Apply(point);
            }

            // Copy UV, N and S vertex data, if present
            if (uv != null) 
            {
                Uv = new Point2F[nbVertices];
                Array.Copy(uv, Uv, nbVertices);
            }
            
            if (n != null) 
            {
                N = new Normal3F[nbVertices];
                for (int i = 0; i < nbVertices; ++i)
                {
                    N[i] = objectToWorld.Apply(n[i]);
                }
            }
            
            if (s != null) 
            {
                S = new Vector3F[nbVertices];
                for (int i = 0; i < nbVertices; ++i)
                {
                    S[i] = objectToWorld.Apply(s[i]);
                }
            }

            if (fIndices != null)
            {
                FaceIndices = new List<int>(fIndices);
            }
        }

        public IEnumerable<IShape> GetTriangles(bool reverseOrientation=false)
        {
            for (int i = 0; i < NbTriangles; ++i)
            {
                yield return new Triangle(ObjectToWorld, WorldToObject, reverseOrientation, this, i);
            }
        }
    }
}