
namespace pbrt.Films
{
    public class Pixel
    {
        public float[] xyz = { 0, 0, 0 };
        public float filterWeightSum = 0;

        public float[] splatXYZ = { 0, 0, 0 };
        public float pad;
 
        public object objLock = new object();
        
        public void AddSplatXyz(float x, float y, float z)
        {
            lock (objLock)
            {
                splatXYZ[0] += x;
                splatXYZ[1] += y;
                splatXYZ[2] += z;
            }
        }
    }
}