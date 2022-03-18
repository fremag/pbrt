using pbrt.Core;

namespace pbrt.Textures
{
    public class PlanarMapping2D : TextureMapping2D
    {
        public Vector3F Vs { get; }
        public Vector3F Vt { get; }
        public float Ds{ get; }
        public float Dt{ get; }

        public PlanarMapping2D(Vector3F vs, Vector3F vt, float ds, float dt)
        {
            Vs = vs;
            Vt = vt;
            Ds = ds;
            Dt = dt;
        }
        
        public override Point2F Map(SurfaceInteraction si, out Vector2F dstdx, out Vector2F dstdy) 
        {
            Vector3F vec = new Vector3F(si.P);
            dstdx = new Vector2F(si.DpDx.Dot(Vs), si.DpDx.Dot(Vt));
            dstdy = new Vector2F(si.DpDy.Dot(Vs), si.DpDy.Dot(Vt));
            return new Point2F(Ds + vec.Dot(Vs), Dt + vec.Dot(Vt));
        }        
    }
}