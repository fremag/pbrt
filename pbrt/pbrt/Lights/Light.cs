using System;
using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Lights
{
    [Flags]
    public enum  LightFlags { DeltaPosition = 1, DeltaDirection = 2, Area = 4, Infinite = 8 };
    
    public abstract class Light
    {
        public LightFlags Flags { get; }
        public Transform LightToWorld { get; }
        public Transform WorldToLight { get; }
        public MediumInterface MediumInterface { get; }
        public int NSamples { get; }

        public abstract Spectrum Power();
        public abstract Spectrum Sample_Li(Interaction interaction, Point2F u, out Vector3F wi, out float pdf, out VisibilityTester vis);
        public abstract float Pdf_Li(Interaction interaction, Vector3F wi);
        
        public Light() // Needed for NSubstitute
        {
        }

        public Light(LightFlags flags, Transform lightToWorld, MediumInterface mediumInterface, int nSamples = 1)
        {
            Flags = flags;
            LightToWorld = lightToWorld;
            MediumInterface = mediumInterface;
            NSamples = nSamples;
            WorldToLight = LightToWorld.Inverse();
            
            // Warn if light has transformation with non-uniform scale
        }

        public virtual void Preprocess(Scene scene)
        {
        }
        
        public static bool IsDeltaLight(LightFlags flags) 
        {
            return flags.HasFlag(LightFlags.DeltaPosition) || flags.HasFlag(LightFlags.DeltaDirection);
        }

        public virtual Spectrum Le(RayDifferential ray)
        {
            return new Spectrum(0f);
        }
    }
}