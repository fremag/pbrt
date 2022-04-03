using pbrt.Core;

namespace pbrt.Shapes
{
    public interface IShape
    {
        Transform ObjectToWorld { get; set; }
        Transform WorldToObject { get; set; }
        bool ReverseOrientation { get; set; }
        bool TransformSwapsHandedness { get; set; }
        float Area { get; }
        Bounds3F ObjectBound { get; }
        Bounds3F WorldBound();
        bool Intersect(Ray ray, out float tHit, out SurfaceInteraction isect, bool testAlphaTexture = true);
        bool IntersectP(Ray ray, bool testAlphaTexture = true);
        Interaction Sample(Point2F u);
        float Pdf(Interaction interaction);
        Interaction Sample(Interaction interaction, Point2F u);
        float Pdf(Interaction interaction, Vector3F wi);
    }
}