namespace pbrt.Core
{
    public interface IMaterial
    {
        void ComputeScatteringFunctions(SurfaceInteraction si, MemoryArena arena, TransportMode mode, bool allowMultipleLobes);
    }
}