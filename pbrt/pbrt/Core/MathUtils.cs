namespace pbrt.Core
{
    public class MathUtils
    {
        public static float Gamma(int n) {
            return (n * float.Epsilon) / (1 - n * float.Epsilon);
        }
    }
}