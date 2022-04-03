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

        public override float Pdf_Li(Interaction interaction, Vector3F wi)
        {
            return Shape.Pdf(interaction, wi);
        }

        public override Spectrum Sample_Li(Interaction interaction, Point2F u, out Vector3F wi, out float pdf, out VisibilityTester vis) 
        {
            Interaction pShape = Shape.Sample(interaction, u);
            pShape.MediumInterface = MediumInterface;
            wi = (pShape.P - interaction.P).Normalized();
            pdf = Shape.Pdf(interaction, wi);
            vis = new VisibilityTester(interaction, pShape);
            return L(pShape, - wi);
        }        
        
        public override Spectrum L(Interaction intr, Vector3F w)
        {
            return intr.N.Dot(w) > 0f ? Lemit : new Spectrum(0f);
        }
    }
}