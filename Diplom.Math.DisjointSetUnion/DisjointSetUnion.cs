namespace Diplom.Math.DisjointSetUnion;

public class DisjointSetUnion<T> where T : IEquatable<T>
{
    private Dictionary<T, T> parent;
    private Dictionary<T, int> rank;
    private int count;
    
    public DisjointSetUnion()
    {
        parent = new Dictionary<T, T>();
        rank = new Dictionary<T, int>();
        count = 0;
    }
    
    public DisjointSetUnion(IEnumerable<T> collection)
    {
        parent = new Dictionary<T, T>();
        rank = new Dictionary<T, int>();
        count = 0;

        foreach (var item in collection)
        {
            MakeSet(item);
        }
    }
    
    
    public int Count => count;
    
    
    public void MakeSet(T item)
    {
        if (!parent.ContainsKey(item))
        {
            parent[item] = item;
            rank[item] = 0;
            count++;
        }
    }
    
    public T Find(T item)
    {
        if (!parent.ContainsKey(item))
        {
            MakeSet(item);
            return item;
        }

        if (!item.Equals(parent[item]))
        {
            parent[item] = Find(parent[item]);
        }
            
        return parent[item];
    }
    
    public void Union(T x, T y)
    {
        T rootX = Find(x);
        T rootY = Find(y);

        if (rootX.Equals(rootY))
            return; 

        if (rank[rootX] < rank[rootY])
        {
            parent[rootX] = rootY;
        }
        else
        {
            parent[rootY] = rootX;
            if (rank[rootX] == rank[rootY])
            {
                rank[rootX]++;
            }
        }

        count--; 
    }
    
    
    public bool AreConnected(T x, T y)
    {
        return Find(x).Equals(Find(y));
    }
    
    public Dictionary<T, List<T>> GetGroups()
    {
        var groups = new Dictionary<T, List<T>>();
            
        foreach (var item in parent.Keys)
        {
            T representative = Find(item);
                
            if (!groups.ContainsKey(representative))
            {
                groups[representative] = new List<T>();
            }
                
            groups[representative].Add(item);
        }
            
        return groups;
    }
    
}