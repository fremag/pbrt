using System.Collections.Generic;

namespace pbrt.Core
{
    public class Distribution2D
    {
        public List<Distribution1D> PConditionalV { get; } = new  List<Distribution1D>();
        public Distribution1D PMarginal { get; }
        
        public Distribution2D(float[] func, int nu, int nv) 
        {
            for (int v = 0; v < nv; ++v)
            {
                // Compute conditional sampling distribution for v  
                var inf = v * nu;
                var sup = inf + nu;
                var values = func[inf .. sup];
                var distribution1D = new Distribution1D(values);
                PConditionalV.Add(distribution1D);
            }
            
            // Compute marginal sampling distribution p(v)
            List<float> marginalFunc = new List<float>();
            for (int v = 0; v < nv; ++v)
            {
                marginalFunc.Add(PConditionalV[v].FuncInt);
            }

            PMarginal = new Distribution1D(marginalFunc.ToArray());
        }
        
        public Point2F SampleContinuous(Point2F u, out float pdf) 
        {
            var u1 = u[1];
            float d1 = PMarginal.SampleContinuous(u1, out var pdfs1, out var v);
            var pConditionalV = PConditionalV[v];
            var u0 = u[0];
            float d0 = pConditionalV.SampleContinuous(u0, out var pdfs0, out _);
            pdf = pdfs0 * pdfs1;
            return new Point2F(d0, d1);
        }
        
        public float Pdf(Point2F p)
        {
            int iu = (int)(p[0] * PConditionalV[0].Count).Clamp( 0, PConditionalV[0].Count - 1);
            int iv = (int)(p[1] * PMarginal.Count).Clamp(0, PMarginal.Count - 1);
            return PConditionalV[iv].Func[iu] / PMarginal.FuncInt;
        }        
    }
}