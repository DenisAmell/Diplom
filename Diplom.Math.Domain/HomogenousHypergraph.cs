
using System.Collections;
using System.Numerics;
using Diplom.Domain.Extensions;
using Diplom.Domain.Helpers;

namespace Diplom.Math.Domain;

public sealed class HomogenousHypergraph :
    IEnumerable<HyperEdge>
{
    private int _verticesCount;
    
    private readonly BitArray _simplices;
    
    private int _simplicesDimension;
    
    private readonly BigInteger _simplicesMaxCount;
    
    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="verticesCount"></param>
    /// <param name="simplicesDimension"></param>
    /// <param name="simplicesVertices"></param>
    public HomogenousHypergraph(
        int verticesCount,
        int simplicesDimension,
        params HyperEdge[]? simplicesVertices)
    {
        Guardant.Instance.ThrowIfGreaterThan(simplicesDimension, verticesCount);
        
        VerticesCount = verticesCount;
        SimplicesDimension = simplicesDimension;
        var simplicesMaxCount = _simplicesMaxCount = BigIntegerExtensions.CombinationsCount(
            VerticesCount, SimplicesDimension);

        
        
        _simplices = new BitArray((int)simplicesMaxCount);
        
        foreach (var simplexVertex in simplicesVertices ?? Enumerable.Empty<HyperEdge>())
        {
            this[simplexVertex] = true;
        }
    }

    #endregion
    
    public int SimplicesMaxCount =>
        (int)_simplicesMaxCount;
    
    private bool this[
        HyperEdge hyperEdge]
    {
        get
        {
            Guardant.Instance
                .ThrowIfNull(hyperEdge)
                .ThrowIfNotEqual(hyperEdge.VerticesCount, SimplicesDimension);
            
            return _simplices[SimplexToBitIndex(hyperEdge)];
        }
        
        set
        {
            Guardant.Instance
                .ThrowIfNull(hyperEdge)
                .ThrowIfNotEqual(hyperEdge.VerticesCount, SimplicesDimension);
            
            _simplices[SimplexToBitIndex(hyperEdge)] = value;
        }
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
    
    
    public int SimplexToBitIndex(
        HyperEdge hyperEdge)
    {
        return SimplexToBitIndex(hyperEdge, SimplicesDimension, VerticesCount, SimplicesMaxCount);
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
    
    
    public bool ContainsSimplex(
        HyperEdge hyperEdge)
    {
        Guardant.Instance
            .ThrowIfNull(hyperEdge);
        
        return this[hyperEdge];
    }
    public bool ContainsSimplex(
        int simplexBitIndex)
    {
        Guardant.Instance
            .ThrowIfLowerThan(simplexBitIndex, 0)
            .ThrowIfGreaterThanOrEqualTo(simplexBitIndex, SimplicesMaxCount);

        return _simplices[simplexBitIndex];
    }
    
    
    
    public static HyperEdge BitIndexToSimplex(
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
        
        return new HyperEdge(result);
    }
    
    public IEnumerable<int> GetSimplicesIncidentToVertexIndices(
        int vertexIndex)
    {
        IEnumerable<HyperEdge> GenerateSimplices(
            int lastAddedVertexIndex,
            HyperEdge toFill)
        {
            if (toFill.VerticesCount == _simplicesDimension)
            {
                yield return toFill;
            }
            else
            {
                for (var i = lastAddedVertexIndex + 1; i < VerticesCount; ++i)
                {
                    if (toFill._vertices.Add(i))
                    {
                        foreach (var generatedSimplex in GenerateSimplices(i, toFill))
                        {
                            yield return generatedSimplex;
                        }

                        toFill._vertices.Remove(i);
                    }
                }
            }
        }

        Guardant.Instance
            .ThrowIfLowerThan(vertexIndex, 0)
            .ThrowIfGreaterThanOrEqualTo(vertexIndex, _verticesCount);

        var simplex = new HyperEdge(vertexIndex);

        foreach (var candidateSimplex in GenerateSimplices(-1, simplex))
        {
            var simplexIndex = SimplexToBitIndex(candidateSimplex);
            if (ContainsSimplex(simplexIndex))
            {
                yield return simplexIndex;
            }
        }
    }
    
    public IEnumerable<HyperEdge> GetSimplicesIncidentToVertex(
        int vertexIndex)
    {
        return GetSimplicesIncidentToVertexIndices(vertexIndex)
            .Select(BitIndexToSimplex);
    }
    
    public IEnumerable<int> GetIncidentVerticesIndicesFor(
        int vertexIndex)
    {
        var result = new HashSet<int>();

        foreach (var simplex in GetSimplicesIncidentToVertex(vertexIndex))
        {
            foreach (var incidentVertexIndex in simplex)
            {
                if (incidentVertexIndex != vertexIndex)
                {
                    if (result.Add(incidentVertexIndex))
                    {
                        yield return incidentVertexIndex;
                    }
                }
            }
        }
    }
    
    public HyperEdge BitIndexToSimplex(
        int simplexBitIndex)
    {
        return BitIndexToSimplex(simplexBitIndex, SimplicesDimension, VerticesCount, SimplicesMaxCount);
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public IEnumerator<HyperEdge> GetEnumerator()
    {
        for (var i = 0; i < SimplicesMaxCount; i++)
        {
            if (!ContainsSimplex(i))
            {
                continue;
            }

            yield return BitIndexToSimplex(i);
        }
    }
    
    
}