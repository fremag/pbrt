using pbrt.Core;

namespace pbrt.Textures
{
    public class TextureMapping3D
    {
        public Transform WorldToTexture { get; }

        public TextureMapping3D(Transform worldToTexture)
        {
            WorldToTexture = worldToTexture;
        }
        
        public Point3F Map(SurfaceInteraction si, out Vector3F dpdx,  out Vector3F dpdy) 
        {
            dpdx = WorldToTexture.Apply(si.DpDx);
            dpdy = WorldToTexture.Apply(si.DpDy);
            return WorldToTexture.Apply(si.P);
        }        
    }
}