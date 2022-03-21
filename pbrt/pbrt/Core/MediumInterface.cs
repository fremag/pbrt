using pbrt.Media;

namespace pbrt.Core
{
    public class MediumInterface
    {
        public Medium Inside { get; set; }
        public Medium Outside { get; set; }

        public MediumInterface(Medium medium) : this(medium, medium)
        { }

        public MediumInterface(Medium inside, Medium outside)
        {
            Inside = inside;
            Outside = outside;
        }

        public bool IsMediumTransition() => Inside != Outside;
    }
}