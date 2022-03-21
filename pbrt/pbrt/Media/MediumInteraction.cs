using pbrt.Core;

namespace pbrt.Media
{
    public class MediumInteraction : Interaction 
    {
        public Medium Medium { get; }
        public PhaseFunction Phase { get; }

        public MediumInteraction(Point3F p, Vector3F wo, float time, Medium medium, PhaseFunction phase)
        {
            P = p;
            Wo = wo;
            Time = time;
            Medium = medium;
            Phase = phase;
        }
    }
}