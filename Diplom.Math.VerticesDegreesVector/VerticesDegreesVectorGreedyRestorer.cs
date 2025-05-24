using System.Numerics;
using System.Runtime.CompilerServices;
using Diplom.Domain.Extensions;
using Diplom.Domain.Helpers;
using Diplom.Math.Domain;

namespace Diplom.Math.VerticesDegreesVector;

public sealed class VerticesDegreesVectorGreedyRestorer
{
    
    private IEnumerable<HomogenousHypergraph?> RestoreAllInnerRecursive(
        Domain.VerticesDegreesVector from,
        int simplicesDimension,
        BigInteger simplicesMaxCount,
        uint verticesDegreesSum,
        ISet<HyperEdge> addedSimplices)
    {
        if (verticesDegreesSum == 0)
        {
            yield return new HomogenousHypergraph(from.VerticesCount, simplicesDimension, addedSimplices.ToArray());
            yield break;
        }

        var lastAddedSimplex = addedSimplices.LastOrDefault();
        var lastAddedSimplexIndex = lastAddedSimplex is null
            ? -1
            : HomogenousHypergraph.SimplexToBitIndex(lastAddedSimplex, simplicesDimension, from.VerticesCount,
                simplicesMaxCount);

        for (var nextSimplexToAddIndex = lastAddedSimplexIndex + 1;
             nextSimplexToAddIndex < simplicesMaxCount;
             nextSimplexToAddIndex++)
        {
            var simplexToAdd = HomogenousHypergraph.BitIndexToSimplex(nextSimplexToAddIndex, simplicesDimension,
                from.VerticesCount, simplicesMaxCount);
            if (!from.TryRemoveSimplex(simplexToAdd))
            {
                continue;
            }

            addedSimplices.Add(simplexToAdd);

            var recursiveRestorations = RestoreAllInnerRecursive(from, simplicesDimension, simplicesMaxCount,
                (uint)(verticesDegreesSum - simplicesDimension), addedSimplices);

            foreach (var recursiveRestoration in recursiveRestorations)
            {
                yield return recursiveRestoration;
            }

            addedSimplices.Remove(simplexToAdd);
            from.AddSimplex(simplexToAdd);
        }
    }
    private async IAsyncEnumerable<HomogenousHypergraph?> RestoreAllInnerRecursiveAsync(
        Domain.VerticesDegreesVector from,
        int simplicesDimension,
        BigInteger simplicesMaxCount,
        uint verticesDegreesSum,
        ISet<HyperEdge> addedSimplices,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (verticesDegreesSum == 0)
        {
            yield return new HomogenousHypergraph(from.VerticesCount, simplicesDimension, addedSimplices.ToArray());
            yield break;
        }

        var lastAddedSimplex = addedSimplices.LastOrDefault();
        var lastAddedSimplexIndex = lastAddedSimplex is null
            ? -1
            : HomogenousHypergraph.SimplexToBitIndex(lastAddedSimplex, simplicesDimension, from.VerticesCount,
                simplicesMaxCount);

        for (var nextSimplexToAddIndex = lastAddedSimplexIndex + 1;
             nextSimplexToAddIndex < simplicesMaxCount;
             nextSimplexToAddIndex++)
        {
            var simplexToAdd = HomogenousHypergraph.BitIndexToSimplex(nextSimplexToAddIndex, simplicesDimension,
                from.VerticesCount, simplicesMaxCount);
            if (!from.TryRemoveSimplex(simplexToAdd))
            {
                continue;
            }

            addedSimplices.Add(simplexToAdd);

            await foreach (var recursiveRestoration in RestoreAllInnerRecursiveAsync(from, simplicesDimension,
                               simplicesMaxCount, (uint)(verticesDegreesSum - simplicesDimension), addedSimplices,
                               cancellationToken))
            {
                yield return recursiveRestoration;
            }

            addedSimplices.Remove(simplexToAdd);
            from.AddSimplex(simplexToAdd);
        }
    }



    private IEnumerable<HomogenousHypergraph?> RestoreAllInnerIterative(
        Domain.VerticesDegreesVector from,
        int simplicesDimension)
    {
        var simplicesMaxCount = BigIntegerExtensions.CombinationsCount(from.VerticesCount, simplicesDimension);
        var simplicesTargetCount = from.Sum(x => x) / simplicesDimension;
        var addedSimplicesSequence = new Stack<(int, HyperEdge)>();
        var addedSimplicesIndices = new HashSet<int>();
        var currentSimplexToAddIndex = 0;

        while (addedSimplicesSequence.Count != simplicesTargetCount)
        {
            if (++currentSimplexToAddIndex == simplicesMaxCount)
            {
                if (addedSimplicesSequence.Count == 0)
                {
                    yield return null;
                }

                addedSimplicesIndices.Remove(currentSimplexToAddIndex = addedSimplicesSequence.Pop().Item1 + 1);
            }

            if (!addedSimplicesIndices.Contains(currentSimplexToAddIndex))
            {
                var simplexToPush = HomogenousHypergraph.BitIndexToSimplex(currentSimplexToAddIndex, simplicesDimension,
                    from.VerticesCount, simplicesMaxCount);
                if (from.TryRemoveSimplex(simplexToPush))
                {
                    addedSimplicesSequence.Push((currentSimplexToAddIndex, simplexToPush));
                    addedSimplicesIndices.Add(currentSimplexToAddIndex);
                }
            }
        }

        yield return new HomogenousHypergraph(from.VerticesCount, simplicesDimension,
            addedSimplicesSequence.Select(x => x.Item2).ToArray());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="simplicesDimension"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete]
    private async IAsyncEnumerable<HomogenousHypergraph?> RestoreAllInnerIterativeAsync(
        Domain.VerticesDegreesVector from,
        int simplicesDimension,
        CancellationToken cancellationToken = default)
    {
        var simplicesMaxCount = BigIntegerExtensions.CombinationsCount(from.VerticesCount, simplicesDimension);
        var simplicesTargetCount = from.Sum(x => x) / simplicesDimension;
        var addedSimplicesSequence = new Stack<(int, HyperEdge)>();
        var addedSimplicesIndices = new HashSet<int>();
        var currentSimplexToAddIndex = -1;
    
        while (addedSimplicesSequence.Count != simplicesTargetCount)
        {
            cancellationToken.ThrowIfCancellationRequested();
    
            while (++currentSimplexToAddIndex == simplicesMaxCount)
            {
                if (addedSimplicesSequence.Count == 0)
                {
                    yield return null;
                }
    
                var poppedSimplex = addedSimplicesSequence.Pop();
                addedSimplicesIndices.Remove(currentSimplexToAddIndex = poppedSimplex.Item1);
                from.AddSimplex(poppedSimplex.Item2);
            }
    
            if (addedSimplicesIndices.Contains(currentSimplexToAddIndex))
            {
                continue;
            }
    
            var simplexToPush = HomogenousHypergraph.BitIndexToSimplex(currentSimplexToAddIndex, simplicesDimension,
                from.VerticesCount, simplicesMaxCount);
            if (!from.TryRemoveSimplex(simplexToPush))
            {
                continue;
            }
    
            addedSimplicesSequence.Push((currentSimplexToAddIndex, simplexToPush));
            addedSimplicesIndices.Add(currentSimplexToAddIndex);
        }
    }
    

 
    protected  VerticesDegreesVectorGreedyRestorer ThrowIfInvalidInputPrototype(
        Domain.VerticesDegreesVector from,
        int simplicesDimension)
    {
        var combinationsCount = BigIntegerExtensions.CombinationsCount(from.VerticesCount - 1, simplicesDimension - 1);
        
        Guardant.Instance
            .ThrowIfNullOrEmpty(from)
            .ThrowIfLowerThan(simplicesDimension, 2)
            .ThrowIfGreaterThan(simplicesDimension, from.VerticesCount)
            .ThrowIfAny(from, vertexDegree => vertexDegree > combinationsCount, "Can't restore homogenous hypergraph: one or more vertices degree is too big.")
            .ThrowIf(from, innerFrom => innerFrom!.Sum(vertexDegree => vertexDegree) % simplicesDimension != 0, "Can't restore homogenous hypergraph: sum of vertices degrees vector components must be divisible by simplices dimension.");

        return this;
    }


    public IEnumerable<HomogenousHypergraph?> RestoreAllInner(
        Domain.VerticesDegreesVector from,
        int simplicesDimension)
    {
        return ThrowIfInvalidInputPrototype(from, simplicesDimension)
                .RestoreAllInnerRecursive(
                    (Domain.VerticesDegreesVector)from.Clone(), 
                    simplicesDimension,
                    BigIntegerExtensions.CombinationsCount(from.VerticesCount, simplicesDimension), 
                    (uint)from.Sum(x => x), 
                    new HashSet<HyperEdge>()
                );
    }
    

    
    public IAsyncEnumerable<HomogenousHypergraph?> RestoreAllInnerAsync(
        Domain.VerticesDegreesVector from,
        int simplicesDimension,
        CancellationToken cancellationToken = default)
    {
        return RestoreAllInnerRecursiveAsync((Domain.VerticesDegreesVector)from.Clone(), simplicesDimension,
            BigIntegerExtensions.CombinationsCount(from.VerticesCount, simplicesDimension), (uint)from.Sum(x => x),
            new HashSet<HyperEdge>(), cancellationToken);
    }

}
