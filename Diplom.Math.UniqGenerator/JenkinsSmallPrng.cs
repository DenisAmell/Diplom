
namespace Diplom.Math.UniqGenerator;

public sealed class JenkinsSmallPrng
{
    private uint a, b, c, d;
    
    public JenkinsSmallPrng(uint seed)
    {
        a = 0xf1ea5eed;
        b = c = d = seed;

        // Прогрев генератора (20 итераций)
        for (var i = 0; i < 20; i++)
        {
            Next();
        }
    }
    
    private uint Next()
    {
        var e = a - ((b << 27) | (b >> 5));
        a = b ^ ((c << 17) | (c >> 15));
        b = c + d;
        c = d + e;
        d = e + a;
        return d;
    }
    
    public uint Next(uint min, uint max)
    {
        if (max <= min)
            throw new ArgumentException("max must be greater than min");

        return min + (Next() % (max - min));
    }
    
    // IEnumerator IEnumerable<int>.GetEnumerator()
    // {
    //     return GetEnumerator();
    // }
    
    public static IEnumerable<int> GenerateUniqueRandom(int min, int max, uint seed)
    {
        var range = max - min;
        var prng = new JenkinsSmallPrng(seed);

        for (var i = 0; i < range; i++)
        {
            // Применяем биективное преобразование
            var permuted = (int)(min + (prng.Next() % (uint)range));
            yield return permuted;
        }
    }

}