namespace pbrt.Core
{
    public class Shading
    {
        public Normal3F N { get; set; }
        public Vector3F DpDu { get; set; }
        public Vector3F DpDv { get; set; }
        public Normal3F DnDu { get; set; }
        public Normal3F DnDv { get; set; }
    }
    
    public class SurfaceInteraction : Interaction 
    {
        public Point2F Uv{ get; set; }
        public Vector3F DpDu{ get; set; }
        public Vector3F DpDv { get; set; }
        public Normal3F DnDu{ get; set; }
        public Normal3F DnDv { get; set; }
        public Shape Shape { get; set; }
        public Shading  Shading { get; set; }
        
        public SurfaceInteraction()
        {
            
        }
        
        public SurfaceInteraction(Point3F p, Vector3F pError, Point2F uv, Vector3F wo,
            Vector3F dpdu, Vector3F dpdv,
            Normal3F dndu, Normal3F dndv,
            float time, Shape shape)
            : base(p, new Normal3F(dpdu.Cross(dpdv).Normalized()), pError, wo, time, null)
        {
            Uv = uv;
            DpDu = dpdu;
            DpDv = dpdv;
            DnDu = dndu;
            DnDv = dndv;
            Shape = shape;

            Shading = new Shading
            {
                N = N,
                DnDu = dndu,
                DnDv = dndv,
                DpDu = dpdu,
                DpDv = dpdv
            };
        }

        public void SetShadingGeometry(Vector3F dpdus, Vector3F dpdvs, Normal3F dndus, Normal3F dndvs, bool orientationIsAuthoritative) {
            Vector3F vector3F = dpdus.Cross(dpdvs).Normalized();
            Shading.N = new Normal3F(vector3F);
            if (Shape != null && (Shape.ReverseOrientation ^ Shape.TransformSwapsHandedness))
            {
                Shading.N = - Shading.N;
            }

            if (orientationIsAuthoritative)
            {
                N = N.FaceForward(Shading.N);
            }
            else
            {
                Shading.N = Shading.N.FaceForward(N);
            }

            Shading.DpDu = dpdus;
            Shading.DpDv = dpdvs;
            Shading.DnDu = dndus;
            Shading.DnDv = dndvs;            
        }        
    }
}