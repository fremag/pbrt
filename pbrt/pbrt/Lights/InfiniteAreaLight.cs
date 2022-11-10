using System;
using System.IO;
using pbrt.Core;
using pbrt.Spectrums;
using pbrt.Textures;

namespace pbrt.Lights;

public class InfiniteAreaLight : Light
{
    public  MipMap Lmap { get; private set;}
    public Point3F WorldCenter { get; private set;}
    public float WorldRadius { get; private set;}
    public Distribution2D Distribution { get; private set; }

    protected InfiniteAreaLight(Transform lightToWorld, int nSamples) : base(LightFlags.Infinite, lightToWorld, null, nSamples)
    {
        
    }
    
    public InfiniteAreaLight(MipMap mipMap, Transform lightToWorld, int nSamples)
        : this(lightToWorld, nSamples)
    {
        Init(mipMap, new Spectrum(1));
    }

    public InfiniteAreaLight(string filename, Stream stream, SampledSpectrum l, Transform lightToWorld, int nSamples = 1) : this(lightToWorld, nSamples)
    {
        var mipMap = ImageTexture.GetTexture(filename, stream, true, 1, ImageWrap.Repeat, 1, true);
        Init(mipMap, l);
    }
    
    private void Init(MipMap mipMap, SampledSpectrum l)
    {
        Lmap = mipMap;

        // Initialize sampling PDFs for infinite area light 
        // Compute scalar - valued image img from environment map 
        int width = mipMap.Resolution.X;
        int  height = mipMap.Resolution.Y;
                
        float filter = 1 / MathF.Max(width, height);
        float[] img = new float[width * height];
        for (int v = 0; v < height; ++v)
        {
            float vp = (float)v / height;
            float sinTheta = MathF.Sin(MathF.PI * (v + .5f) / height);
            for (int u = 0; u < width; ++u)
            {
                float up = (float)u / width;
                var rgbSpectrum = Lmap.Lookup(new Point2F(up, vp), filter);
                var floats = rgbSpectrum.ToRgb();
                var sampledSpectrum = SampledSpectrum.FromRgb(floats, SpectrumType.Illuminant);
                var spectrum = new Spectrum(sampledSpectrum);
                spectrum.Mul(l);
                
                img[u + v * width] = spectrum.Y();
                img[u + v * width] *= sinTheta;
            }
        }

        // Compute sampling distributions for rows and columns of image 
        Distribution = new Distribution2D(img, width, height);
    }

    public override Spectrum Power()
    {
        var rgbSpectrum = Lmap.Lookup(new Point2F(.5f, .5f), .5f);
        var f = MathF.PI * WorldRadius * WorldRadius;
        rgbSpectrum.Mul(f);
        return new Spectrum(rgbSpectrum, SpectrumType.Illuminant);
    }

    public override Spectrum Le(RayDifferential ray)
    {
        var w = WorldToLight.Apply(ray.D).Normalized();
        Point2F st = new(MathUtils.SphericalPhi(w) * MathUtils.Inv2PI, MathUtils.SphericalTheta(w) * MathUtils.InvPI);
        return new Spectrum(Lmap.Lookup(st, 0), SpectrumType.Illuminant);
    }    

    public override Spectrum Sample_Li(Interaction interaction, Point2F u, out Vector3F wi, out float pdf, out VisibilityTester vis)
    {
        // Find sample coordinates in infinite light texture 
        var uv = Distribution.SampleContinuous(u, out var mapPdf);
        if (mapPdf == 0)
        {
            wi = null;
            pdf = 0;
            vis = null;
            return new Spectrum(0f);
        }

        // Convert infinite light sample point to direction 
        float theta = uv[1] * MathF.PI;
        float phi = uv[0] * 2 * MathF.PI;
        float cosTheta = MathF.Cos(theta);
        float sinTheta = MathF.Sin(theta);
        float sinPhi = MathF.Sin(phi);
        float cosPhi = MathF.Cos(phi);
        wi = LightToWorld.Apply(new Vector3F(sinTheta * cosPhi, sinTheta * sinPhi, cosTheta));
            
        // Compute PDF for sampled infinite light direction>> 
        pdf = mapPdf / (2 * MathF.PI * MathF.PI * sinTheta);
        if (sinTheta == 0)
        {
            pdf = 0;
        }

        // Return radiance value for infinite light direction 
        vis = new VisibilityTester(interaction, new Interaction(interaction.P + wi * (2 * WorldRadius), interaction.Time, MediumInterface));
        return new Spectrum(Lmap.Lookup(uv, 0), SpectrumType.Illuminant);    
    }

    public override float Pdf_Li(Interaction interaction, Vector3F wi)
    {
        Vector3F w = WorldToLight.Apply(wi);
        float theta = MathUtils.SphericalTheta(wi), phi = MathUtils.SphericalPhi(wi);
        float sinTheta = MathF.Sin(theta);
        if (sinTheta == 0) return 0;
        return Distribution.Pdf(new Point2F(phi * MathUtils.Inv2PI, theta * MathUtils.InvPI)) / (2 * MathF.PI * MathF.PI * sinTheta);
    }

    public override void Preprocess(Scene scene)
    {
        scene.WorldBound.BoundingSphere(out var worldCenter, out var worldRadius);
        WorldCenter = worldCenter;
        WorldRadius = worldRadius;
    }
}