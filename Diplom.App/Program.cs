
using Diplom.Math.Domain;
using Diplom.Math.VerticesDegreesVector;


public class Program
{
    public static void Main()
    {
        
        var vector = new VerticesDegreesVector(4, 4, 4, 1, 1, 1, 1);
        
        var result = new VerticesDegreesVectorGreedyRestorer().RestoreAllInner(vector, 4);
        
        foreach (var hypergraph in result)
        {
            if (hypergraph != null)
            {
                Console.WriteLine($"VerticesCount: {hypergraph.VerticesCount}");
                Console.WriteLine($"SimplicesDimension: {hypergraph.SimplicesDimension}");
                var hyperEdges = hypergraph.GetEnumerator();

                while (hyperEdges.MoveNext())
                {
                    var edge = hyperEdges.Current;
                    Console.WriteLine($"edge: {edge.ToString()}");
                }
          
                Console.WriteLine(); 
            }
            else
            {
                Console.WriteLine("Null element encountered.");
            }
        }
    }
}