using System;
using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Lights
{
    public class PointLight : Light
    {
        public Point3F PLight { get; }
        public Spectrum I { get; }
        
        public PointLight(Transform lightToWorld, MediumInterface mediumInterface, Spectrum i)
            : base(LightFlags.DeltaPosition, lightToWorld, mediumInterface)
        {
            I = i;
            PLight = LightToWorld.Apply(Point3F.Zero);
        }
        
        public override Spectrum Power() => 4f * MathF.PI * I;

        public override Spectrum Sample_Li(Interaction interaction, Point2F u, out Vector3F wi, out float pdf, out VisibilityTester vis) 
        {
            wi = (PLight - interaction.P).Normalized();
            pdf = 1f;
            vis = new VisibilityTester(interaction, new Interaction(PLight, interaction.Time, MediumInterface));
            return I / Point3F.DistanceSquared(PLight, interaction.P);
        }
        
        public override float Pdf_Li(Interaction interaction, Vector3F wi)
        {
            return 0;
        }        
    }
}