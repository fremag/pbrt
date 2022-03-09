using System;
using pbrt.Spectrums;

namespace pbrt.Reflections
{
    public class FresnelConductor : Fresnel
    {
        public Spectrum EtaI { get; }
        public Spectrum EtaT { get; }
        public Spectrum K { get; }

        public FresnelConductor(Spectrum etaI, Spectrum etaT, Spectrum k)
        {
            EtaI = etaI;
            EtaT = etaT;
            K = k;
        }

        public Spectrum Evaluate(float cosI) => BxDF.FrConductor(MathF.Abs(cosI), EtaI, EtaT, K);
    }
}