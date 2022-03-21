using System;

namespace pbrt.Lights
{
    public static class LightUtils
    {
        // speed light
        const float c = 299792458f;

        // Planck
        const float h = 6.62606957e-34f;

        // Boltzmann
        const float kb = 1.3806488e-23f;

        // Wien displacement constant (m . K)
        public const float Wien = 2.8977721e-3f;

        public static void BlackBody(float[] lambda, int n, float t, float[] le)
        {
            for (int i = 0; i < n; ++i)
            {
                // Compute emitted radiance for blackbody at wavelength lambda[i]
                float l = lambda[i] * 1e-9f;
                float lambda5 = (l * l) * (l * l) * l;
                le[i] = (2 * h * c * c) / (lambda5 * (MathF.Exp((h * c) / (l * kb * t)) - 1));
            }
        }

        public static void BlackBodyNormalized(float[] lambda, int n, float t, float[] le)
        {
            BlackBody(lambda, n, t, le);
            // Normalize Le values based on maximum blackbody radiance
            float[] lambdaMax = new float[] { Wien / t * 1e9f };
            float[] maxL = new float[1];
            BlackBody(lambdaMax, 1, t, maxL);
            for (int i = 0; i < n; ++i)
            {
                le[i] /= maxL[0];
            }
        }
    }
}