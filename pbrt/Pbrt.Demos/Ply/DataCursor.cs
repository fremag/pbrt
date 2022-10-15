using System.Collections;

namespace Pbrt.Demos.Ply;

public class DataCursor
{
    public IList Vector { get; }
    public bool IsMultiVector { get; }
    
    public DataCursor(IList vector, bool isMultiVector=false)
    {
        Vector = vector;
        IsMultiVector = isMultiVector;
    }
}