using System.Diagnostics;
using pbrt.Lights;
using pbrt.Samplers;
using pbrt.Spectrums;

namespace pbrt.Core
{
    public interface IScene
    {
        IPrimitive Aggregate { get; }
        Light[] Lights { get; }
        Bounds3F WorldBound { get; }

        bool IntersectP(Ray ray);
        bool Intersect(Ray ray, out SurfaceInteraction isect);
        bool IntersectTr(Ray ray, ISampler sampler, out SurfaceInteraction isect, out Spectrum tr);
        void Init();
    }

    public class Scene : IScene
    {
        public IPrimitive Aggregate { get; private set; }
        public Light[] Lights { get;  private set;}
        public Bounds3F WorldBound { get;  private set;}

        public Scene()
        {
        }
        
        public virtual void Init() { }

        public Scene(IPrimitive aggregate, Light[] lights)
        {
            Init(aggregate, lights);
        }

        protected void Init(IPrimitive aggregate, Light[] lights)
        {
            Aggregate = aggregate;
            Lights = lights;
            // Scene Constructor Implementation 
            WorldBound = aggregate.WorldBound();
            foreach (var light in Lights)
            {
                light.Preprocess(this);
            }
        }

        public bool IntersectP(Ray ray)
        {
            return Aggregate.IntersectP(ray);
        }

        public bool Intersect(Ray ray, out SurfaceInteraction isect)
        {
            var intersect = Aggregate.Intersect(ray, out isect);
            return intersect;
        }
        
        public bool IntersectTr(Ray ray, ISampler sampler, out SurfaceInteraction isect, out Spectrum tr) 
        {
            tr = new Spectrum(1f);
            while (true) 
            {
                bool hitSurface = Intersect(ray, out isect);
                // Accumulate beam transmittance for ray segment 
                if (ray.Medium != null)
                {
                    var spectrum = ray.Medium.Tr(ray, sampler);
                    tr *= spectrum;
                }
                
                // Initialize next ray segment or terminate transmittance computation 
                if (!hitSurface)
                {
                    return false;
                }

                if (isect.Primitive.GetMaterial() != null)
                {
                    return true;
                }

                ray = isect.SpawnRay(ray.D);                
            }
        }
    }
}