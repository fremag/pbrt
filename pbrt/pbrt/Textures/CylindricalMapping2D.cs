using System;
using pbrt.Core;

namespace pbrt.Textures
{
    public class CylindricalMapping2D : TextureMapping2D 
    {
        public Transform WorldToTexture { get; }
    
        public CylindricalMapping2D(Transform worldToTexture)
        {
            WorldToTexture = worldToTexture;
        }

        public override Point2F Map(SurfaceInteraction si, out Vector2F dstdx, out Vector2F dstdy)
        {
            Point2F st = Cylinder(si.P);
            // Compute texture coordinate differentials for cylinder (u, v) mapping>> 
            float delta = .01f;
            Point2F stDeltaX = Cylinder(si.P + delta * si.DpDx);
            dstdx = (stDeltaX - st) / delta;
            if (dstdx.Y > .5)
            {
                dstdx.Y = 1f - dstdx.Y;
            }
            else if (dstdx.Y < -.5f)
            {
                dstdx.Y = -(dstdx.Y + 1);
            }

            Point2F stDeltaY = Cylinder(si.P + delta * si.DpDy);
            dstdy = (stDeltaY - st) / delta;
            if (dstdy.Y > .5)
            {
                dstdy.Y = 1f - dstdy.Y;
            }
            else if (dstdy.Y < -.5f)
            {
                dstdy.Y = -(dstdy.Y + 1);
            }

            return st;            
        }
        
        public Point2F Cylinder(Point3F p) 
        {
            Vector3F vec = (WorldToTexture.Apply(p) - Point3F.Zero).Normalized();
            return new Point2F((MathF.PI + MathF.Atan2(vec.Y, vec.X)) / (2 * MathF.PI), vec.Z);
        }        
    }
}