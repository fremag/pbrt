using System;
using pbrt.Core;

namespace pbrt.Textures
{
    public class SphericalMapping2D : TextureMapping2D 
    {
        public Transform WorldToTexture { get; }

        public SphericalMapping2D(Transform worldToTexture)
        {
            WorldToTexture = worldToTexture;
        }
        
        public override Point2F Map(SurfaceInteraction si, out Vector2F dstdx, out Vector2F dstdy)
        {
            Point2F st = Sphere(si.P);
            // Compute texture coordinate differentials for sphere (u, v) mapping 
            float delta = .1f;
            Point2F stDeltaX = Sphere(si.P + delta * si.DpDx);
            dstdx = (stDeltaX - st) / delta;
            Point2F stDeltaY = Sphere(si.P + delta * si.DpDy);
            dstdy = (stDeltaY - st) / delta;   
            
            if (dstdx[1] > .5)
            {
                dstdx.Y = 1 - dstdx[1];
            }
            else if (dstdx[1] < -.5f)
            {
                dstdx.Y = -(dstdx[1] + 1);
            }

            if (dstdy[1] > .5)
            {
                dstdy.Y = 1 - dstdy[1];
            }
            else if (dstdy[1] < -.5f)
            {
                dstdy.Y = -(dstdy[1] + 1);
            }
        
            return st;
        }
        
        public Point2F Sphere(Point3F p) 
        {
            Vector3F vec = (WorldToTexture.Apply(p) - Point3F.Zero).Normalized();
            float theta = MathUtils.SphericalTheta(vec);
            float phi = MathUtils.SphericalPhi(vec);
            return new Point2F(theta / MathF.PI, phi /(2 * MathF.PI));
        }
    }
}