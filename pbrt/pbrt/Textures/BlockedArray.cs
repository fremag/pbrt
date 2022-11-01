namespace pbrt.Textures;

public class BlockedArray<T> where T : new()
{
    public T[] Data { get; }
    public int URes { get; }
    public int VRes { get; } 
    public int UBlocks { get; }
    public int LogBlockSize { get; }
    
    // BlockedArray Public Methods
    public BlockedArray(int uRes, int vRes, T[] d = null, int logBlockSize=2)
    {
        URes = uRes;
        VRes = vRes;
        UBlocks = RoundUp(uRes) >> logBlockSize;
        LogBlockSize = logBlockSize;
        
        int nAlloc = RoundUp(uRes) * RoundUp(vRes);
        Data = new T[nAlloc];
        for (int i = 0; i < nAlloc; ++i)
        {
            Data[i] = new T();
        }

        if (d == null)
        {
            return;
        }

        for (int v = 0; v < vRes; ++v)
        {
            for (int u = 0; u < uRes; ++u)
            {
                this[u, v] = d[v * uRes + u];
            }
        }
    }

    public int BlockSize()
    {
        return 1 << LogBlockSize;
    }
    
    public int RoundUp(int x) 
    {
        return (x + BlockSize() - 1) & ~(BlockSize() - 1);
    }
    
    public int USize() => URes;
    public int VSize() => VRes;

    public int Block(int a) => a >> LogBlockSize;
    public int Offset(int a) => a & (BlockSize() - 1);
    
    public T this[int u, int v] 
    {
        get
        {
            var bu = Block(u);
            var bv = Block(v);
            var ou = Offset(u);
            var ov = Offset(v);
            var blockSize = BlockSize();
            var blockSize2 = blockSize * blockSize;
            int offset = blockSize2 * (UBlocks * bv + bu);
            offset += blockSize * ov + ou;
            return Data[offset];
        }

        set
        {
            var bu = Block(u);
            var bv = Block(v);
            var ou = Offset(u);
            var ov = Offset(v);
            int offset = BlockSize() * BlockSize() * (UBlocks * bv + bu);
            offset += BlockSize() * ov + ou;
            Data[offset] = value;
        }
    }

    public void GetLinearArray(T[] a)
    {
        int i = 0;
        for (int v = 0; v < VRes; ++v)
        {
            for (int u = URes - 1; u >= 0; --u, i++)
            {
                a[i] = this[u, v];
            }
        }
    }
};