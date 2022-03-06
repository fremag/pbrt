using pbrt.Core;

namespace pbrt.Filters
{
    public class BoxFilter : AbstractFilter
    {
        public BoxFilter(Vector2F radius) : base(radius)
        {
        }

        public override float Evaluate(Point2F p) => 1f;
    }
}