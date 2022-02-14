using pbrt.Core;

namespace pbrt.Shapes
{
    public abstract class AbstractShape 
    {
        public Transform ObjectToWorld { get; set; }
        public Transform WorldToObject { get; set; }
        public bool ReverseOrientation { get; set; }
        public bool TransformSwapsHandedness { get; set; }
        
        public AbstractShape(Transform objectToWorld,
        Transform worldToObject, bool reverseOrientation)
        {
            ObjectToWorld = objectToWorld;
            WorldToObject = worldToObject;
            ReverseOrientation = reverseOrientation;
            TransformSwapsHandedness = ObjectToWorld.SwapsHandedness();
        }
        
        public float Area { get; protected set; }
        public Bounds3F ObjectBound { get; protected set; }
        public Bounds3F WorldBound() => ObjectToWorld.Apply(ObjectBound);
        
        public abstract bool Intersect(Ray ray, out float tHit, out SurfaceInteraction isect, bool testAlphaTexture = true);
        public virtual bool IntersectP(Ray ray, bool testAlphaTexture = true) => Intersect(ray, out _, out _, testAlphaTexture);
    }
}