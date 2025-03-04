using System.Numerics;

using Diplom.Domain.Helpers;

namespace Diplom.Domain.Extensions;

public static class BigIntegerExtensions
{
    
    public static BigInteger Two =>
        2;
    
    public static BigInteger LeastCommonMultiple(
        BigInteger left,
        BigInteger right)
    {
        return BigInteger.Abs(left * right) / BigInteger.GreatestCommonDivisor(left, right);
    }
    
    public static BigInteger Factorial(
        this BigInteger value)
    {
        Guardant.Instance
            .ThrowIfLowerThan(value, BigInteger.Zero);

        var factorial = BigInteger.One;

        for (var i = Two; i <= value; i++)
        {
            factorial *= i;
        }

        return factorial;
    }
    
    public static BigInteger CombinationsCount(
        BigInteger n,
        BigInteger k)
    {
        Guardant.Instance
            .ThrowIfLowerThan(n, BigInteger.Zero);

        if (n == k)
        {
            return BigInteger.One;
        }

        if (k == BigInteger.One)
        {
            return n;
        }

        if (n < k)
        {
            return BigInteger.Zero;
        }

        var partialResult = BigInteger.One;
        for (BigInteger i = n - k + 1; i <= n; i++)
        {
            partialResult *= i;
        }
        
        return partialResult / k.Factorial();
    }
}