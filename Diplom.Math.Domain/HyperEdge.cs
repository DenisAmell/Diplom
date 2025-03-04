using System.Collections;

using Diplom.Domain.Helpers;


namespace Diplom.Math.Domain;

public sealed class HyperEdge: 
    IEquatable<HyperEdge>,
    IEnumerable<int>
{
 
    internal readonly SortedSet<int> _vertices;
    
    public HyperEdge(
        params int[] values)
    {
        Guardant.Instance
            .ThrowIfNullOrEmpty(values);

        _vertices = new SortedSet<int>(values);
    }
    
    public int VerticesCount =>
        _vertices.Count;
    
    public override bool Equals(
        object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj is HyperEdge hyperEdge)
        {
            return Equals(hyperEdge);
        }

        return false;
    }
    
    /// <inheritdoc cref="object.GetHashCode" />
    public override int GetHashCode()
    {
        var combinedHashCode = default(HashCode);
        
        foreach (var vertexIndex in _vertices)
        {
            combinedHashCode.Add(vertexIndex.GetHashCode());
        }

        return combinedHashCode.ToHashCode();
    }

    /// <inheritdoc cref="object.ToString" />
    public override string ToString()
    {
        return string.Join(", ", _vertices);
    }
    

    public bool Equals(
        HyperEdge? hyperEdge)
    {
        if (hyperEdge is null)
        {
            return false;
        }

        return _vertices.Equals(hyperEdge._vertices);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public IEnumerator<int> GetEnumerator()
    {
        return _vertices.GetEnumerator();
    }

    
}