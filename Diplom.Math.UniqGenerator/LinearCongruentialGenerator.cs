public class LinearCongruentialGenerator
{
    private long _seed;
    
    private const long a = 1664525;
    private const long c = 1013904223;
    private const long m = 4294967296; // 2^32
        
    public LinearCongruentialGenerator(long seed)
    {
        _seed = seed;
    }
        
    public long Next()
    {
        _seed = (a * _seed + c) % m;
        return _seed;
    }
        
 
    public double NextDouble()
    {
        return (double)Next() / m;
    }
}
