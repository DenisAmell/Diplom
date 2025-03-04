

public sealed class HomogenousHypergraph : IEnumerable<HyperEdge>
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
        Guardant.Instance
            .ThrowIfGreaterThan(simplicesDimension, verticesCount);
        
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
}