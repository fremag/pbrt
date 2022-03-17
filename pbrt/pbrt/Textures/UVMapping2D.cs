using pbrt.Core;

namespace pbrt.Textures
{
    public class UVMapping2D : TextureMapping2D
    {
        float Su { get; set; }
        float Sv { get; set; }
        float Du { get; set; }
        float Dv { get; set; }

        public UVMapping2D(float su, float sv, float du, float dv)
        {
            Su = su;
            Sv = sv;
            Du = du;
            Dv = dv;
        }

        public override Point2F Map(SurfaceInteraction si, out Vector2F dstdx, out Vector2F dstdy)
        {
            // Compute texture differentials for 2D (u, v) mapping 
            dstdx = new Vector2F(Su * si.DuDx, Sv * si.DvDx);
            dstdy = new Vector2F(Su * si.DuDy, Sv * si.DvDy);
            
            return new Point2F(Su * si.Uv[0] + Du, Sv * si.Uv[1] + Dv);
        }
    }
}