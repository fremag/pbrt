using pbrt.Core;

namespace pbrt.Filters
{
    public abstract class AbstractFilter
    {
        public Vector2F Radius { get; }
        public Vector2F InvRadius { get; set; }

        public AbstractFilter(Vector2F radius)
        {
            Radius = radius;
            InvRadius = new Vector2F(1 / radius.X, 1 / radius.Y);
        }
        
        public abstract float Evaluate(Point2F p);
    }
}