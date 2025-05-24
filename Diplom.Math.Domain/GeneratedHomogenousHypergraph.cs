using System.Numerics;
using Diplom.Domain.Extensions;
using Diplom.Domain.Helpers;
using Diplom.Math.UniqGenerator;
using Diplom.Math.DisjointSetUnion;
namespace Diplom.Math.Domain;

public class GeneratedHomogenousHypergraph
{
    
    private int _verticesCount;
    
    private int _simplicesDimension;

    private readonly BigInteger _simplicesMaxCount;

    private int[] _vertices;
   
    public GeneratedHomogenousHypergraph(
        int verticesCount,
        int simplicesDimension
    )
    {
        Guardant.Instance.ThrowIfGreaterThan(simplicesDimension, verticesCount);
        
        VerticesCount = verticesCount;
        SimplicesDimension = simplicesDimension;
        var simplicesMaxCount = _simplicesMaxCount = BigIntegerExtensions.CombinationsCount(
            VerticesCount, SimplicesDimension);

        _vertices = new int[VerticesCount];
        
        for (int i = 0; i < verticesCount; i++)
        {
            _vertices[i] = i;
        }
        
    }

    public int SimplicesMaxCount =>
        (int)_simplicesMaxCount;


    public int[] Vertices => _vertices;

    public HomogenousHypergraph generateKey(
        int g,
        int p
    )
    {
        
        var K = GenerateDiffieHellmanKey(g, p);
        
        var lcg = new LinearCongruentialGenerator((long)(K % long.MaxValue));
        
        bool[] edgeExists = new bool[SimplicesMaxCount];
        
        for (int i = 0; i < SimplicesMaxCount; i++)
        {
            edgeExists[i] = lcg.NextDouble() < 0.5;
        }
        
        var dsu = new DisjointSetUnion<int>(Vertices);
        
        var newHybereges = MakeConnected(dsu, edgeExists);

        foreach (var hyberege in newHybereges)
        {
            var index = SimplexToBitIndex(new HyperEdge(hyberege), SimplicesDimension, VerticesCount, SimplicesMaxCount);
            edgeExists[index] = true;
        }

        var hypereges = new List<HyperEdge>();

        for (int i = 0; i < edgeExists.Length; i++)
        {
            if (edgeExists[i])
            {
                hypereges.Add(new HyperEdge(BitIndexToSimplex(i, SimplicesDimension,VerticesCount,  SimplicesMaxCount)));
            }
        }

        var hypergraph = new HomogenousHypergraph(
            VerticesCount,
            SimplicesDimension,
            hypereges.ToArray()
        );
        
        return hypergraph;

    }
    
    public List<int[]> MakeConnected(DisjointSetUnion<int> dsu,  bool[] edgeExists)
    {
    
        for (int i = 0; i < SimplicesMaxCount; i++)
        {
            if (edgeExists[i])
            {
           
                int[] vertices = BitIndexToSimplex(i, SimplicesDimension, VerticesCount, SimplicesMaxCount);
                
                for (int j = 1; j < vertices.Length; j++)
                {
                    dsu.Union(vertices[0], vertices[j]);
                }
            }
        }
        
        if (dsu.Count == 1)
        {
            return new List<int[]>();
        }
        
         return AddMissingHyperedges(dsu,edgeExists);
    }
    
    
    private List<int[]> AddMissingHyperedges(DisjointSetUnion<int> dsu,  bool[] edgeExists)
    {
        List<int[]> addedHyperedges = new List<int[]>();
        Random rand = new Random();

        while (dsu.Count > 1)
        {
            int randomIndex;
            int[] hyperedge;
            do
            {
                randomIndex = rand.Next(0, SimplicesMaxCount);
                hyperedge = BitIndexToSimplex(randomIndex, SimplicesDimension, VerticesCount, SimplicesMaxCount);
            } while (edgeExists[randomIndex]);

            addedHyperedges.Add(hyperedge);
            
            for (int j = 1; j < hyperedge.Length; j++)
            {
                dsu.Union(hyperedge[0], hyperedge[j]);
            }
        }

        foreach (var hyperedge in addedHyperedges)
        {
            Console.WriteLine(string.Join(", ", hyperedge));
        }

      
        
        return addedHyperedges;
    }
    
    
    
    private BigInteger GenerateDiffieHellmanKey(BigInteger g, BigInteger p)
    {
        
        var random = new Random();
        byte[] bytes = new byte[16];
        
        random.NextBytes(bytes);
        BigInteger a = new BigInteger(bytes);
        a = BigInteger.Abs(a);
            

        BigInteger A = BigInteger.ModPow(g, a, p);
        random.NextBytes(bytes);
        
        BigInteger b = new BigInteger(bytes);
        b = BigInteger.Abs(b);
        
        BigInteger B = BigInteger.ModPow(g, b, p);
        BigInteger K = BigInteger.ModPow(B, a, p);
        
        return K;
    }
    
    
    public int VerticesCount
    {
        get =>
            _verticesCount;

        private set
        {
            Guardant.Instance
                .ThrowIfLowerThanOrEqualTo(value, 0);

            _verticesCount = value;
        }
    }
    
    public int SimplicesDimension
    {
        get =>
            _simplicesDimension;

        private set
        {
            Guardant.Instance
                .ThrowIfLowerThanOrEqualTo(value, 1);

            _simplicesDimension = value;
        }
    }
    
    public int[] BitIndexToSimplex(
        int simplexBitIndex,
        int simplicesDimension,
        int verticesCount,
        BigInteger simplicesMaxCount)
    {
        Guardant.Instance
            .ThrowIfLowerThan(simplexBitIndex, 0)
            .ThrowIfGreaterThanOrEqualTo(simplexBitIndex, simplicesMaxCount);
        
        var result = new int[simplicesDimension];
        var r = (BigInteger)simplexBitIndex;
        var j = 0;
        
        for (var i = 0; i < simplicesDimension; i++)
        {
            var cs = j + 1;
            BigInteger cc;
            
            while (r - (cc = BigIntegerExtensions.CombinationsCount(verticesCount - cs, simplicesDimension - i - 1)) >= 0)
            {
                r -= cc;
                cs++;
            }

            result[i] = cs - 1;
            j = cs;
        }
        
        return result;
    }
    
    
    public static int SimplexToBitIndex(
        HyperEdge hyperEdge,
        int simplicesDimension,
        int verticesCount,
        BigInteger simplicesMaxCount)
    {
        Guardant.Instance
            .ThrowIfNull(hyperEdge);

        var result = BigInteger.Zero;
        using var enumerator = hyperEdge.GetEnumerator();
        for (var i = 0; i < simplicesDimension; i++)
        {
            if (!enumerator.MoveNext())
            {
                // TODO: ?!
            }
            
            var value = enumerator.Current;

            result += BigIntegerExtensions.CombinationsCount(verticesCount - value - 1, simplicesDimension - i);
        }

        return (int)(simplicesMaxCount - 1 - result);
    }
    


}