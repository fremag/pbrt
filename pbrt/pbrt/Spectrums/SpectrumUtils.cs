namespace pbrt.Spectrums
{
    public class SpectrumUtils
    {
        
        public static float[] XYZToRGB(float[] xyz) 
        {
            float[] rgb = new float[3];
            rgb[0] =  3.240479f*xyz[0] - 1.537150f*xyz[1] - 0.498535f*xyz[2];
            rgb[1] = -0.969256f*xyz[0] + 1.875991f*xyz[1] + 0.041556f*xyz[2];
            rgb[2] =  0.055648f*xyz[0] - 0.204043f*xyz[1] + 1.057311f*xyz[2];
            return rgb;
        }
        
        public static float[] RGBToXYZ(float[] rgb) 
        {
            float[] xyz = new float[3];
            xyz[0] = 0.412453f*rgb[0] + 0.357580f*rgb[1] + 0.180423f*rgb[2];
            xyz[1] = 0.212671f*rgb[0] + 0.715160f*rgb[1] + 0.072169f*rgb[2];
            xyz[2] = 0.019334f*rgb[0] + 0.119193f*rgb[1] + 0.950227f*rgb[2];
            return xyz;
        }
    }
}