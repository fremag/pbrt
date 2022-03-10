using System;
using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Reflections
{
    public class FresnelSpecular : BxDF 
    {
        public Spectrum R { get; }
        public Spectrum T { get; }
        public TransportMode Mode { get; }
        public Fresnel Fresnel { get; }
        
        public FresnelSpecular(Spectrum r, Spectrum t, float etaA, float etaB, TransportMode mode)
            : base(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_SPECULAR)
        {
            R = r;
            T = t;
            Mode = mode;
            Fresnel = new FresnelDielectric(etaA, etaB);
        }
        
        public override Spectrum F(Vector3F wo, Vector3F wi) => new Spectrum(0f);
        
        public override Spectrum Sample_f(Vector3F wo, out Vector3F wi, Point2F sample, out float pdf, out BxDFType sampledType)
        {
            // the implementation of the Sample_f() method is in Section 14.1.3. 
            // https://pbr-book.org/3ed-2018/Light_Transport_I_Surface_Reflection/Sampling_Reflection_Functions.html#sec:mc-specular-deltas
            throw new NotImplementedException();
        }        
    };
}