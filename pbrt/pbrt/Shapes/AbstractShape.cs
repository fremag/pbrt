using System.Diagnostics;
using pbrt.Core;

namespace pbrt.Shapes
{
    public abstract class AbstractShape : IShape
    {
        public Transform ObjectToWorld { get; set; }
        public Transform WorldToObject { get; set; }
        public bool ReverseOrientation { get; set; }
        public bool TransformSwapsHandedness { get; set; }
        public abstract float Area { get; }
    
        public AbstractShape(Transform objectToWorld, Transform worldToObject, bool reverseOrientation)
        {
            ObjectToWorld = objectToWorld;
            WorldToObject = worldToObject;
            ReverseOrientation = reverseOrientation;
            TransformSwapsHandedness = ObjectToWorld.SwapsHandedness();
        }
        
        public Bounds3F ObjectBound { get; protected set; }
        public Bounds3F WorldBound() => ObjectToWorld.Apply(ObjectBound);
        
        public abstract bool Intersect(Ray ray, out float tHit, out SurfaceInteraction isect, bool testAlphaTexture = true);
        public virtual bool IntersectP(Ray ray, bool testAlphaTexture = true) => Intersect(ray, out _, out _, testAlphaTexture);
        public abstract Interaction Sample(Point2F u);
        
        public virtual float Pdf(Interaction interaction) => 1 / Area;
        public virtual Interaction Sample(Interaction interaction, Point2F u) => Sample(u);

        public virtual float Pdf(Interaction interaction, Vector3F wi) 
        {
            // Intersect sample ray with area light geometry 
            Ray ray = interaction.SpawnRay(wi);

            if (!Intersect(ray, out _, out var isectLight, false))
            {
                return 0;
            }
            
            // Convert light sample weight to solid angle measure
            float pdf = interaction.P.DistanceSquared(isectLight.P) / (isectLight.N.AbsDot(-wi) * Area);            
            return pdf;
        }        
    }
}