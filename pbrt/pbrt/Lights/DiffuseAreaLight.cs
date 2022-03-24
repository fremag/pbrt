using System;
using pbrt.Core;
using pbrt.Shapes;
using pbrt.Spectrums;

namespace pbrt.Lights
{
    public class DiffuseAreaLight : AreaLight
    {
        public float Area => Shape.Area;
        public IShape Shape { get; }
        public Spectrum Lemit { get; }

        public DiffuseAreaLight(Transform lightToWorld, MediumInterface mediumInterface, Spectrum lemit, int nSamples, IShape shape)
            : base(LightFlags.Area, lightToWorld, mediumInterface, nSamples)
        {
            Lemit = lemit;
            Shape = shape;
        }

        public override Spectrum Power()
        {
            return Lemit * Area * MathF.PI;
        }

        public override Spectrum Sample_Li(Interaction interaction, Point2F u, out Vector3F wi, out float pdf, out VisibilityTester vis)
        {
            wi = null;
            pdf = 1;
            vis = null;
            // We will defer answering this question and providing an implementation of this method until Section 14.2, after Monte Carlo integration has been introduced. 
            throw new NotImplementedException();
        }

        public override Spectrum L(Interaction intr, Vector3F w)
        {
            return intr.N.Dot(w) > 0f ? Lemit : new Spectrum(0f);
        }
    }
}