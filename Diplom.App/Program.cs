
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

        
        var key = new GeneratedHomogenousHypergraph(5, 3).generateKey(23, 5);
        
        var algorithm = new HomogenousHypergraphEncryptor(key, 2);
        
        
        var block = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        Console.Write("Initial state: ");
        for (var i = 0; i < block.Length; ++i)
        {
            Console.Write($"{block[i]} ");
        }
        Console.WriteLine();

        CryptoTransformationContext.PerformCipher(algorithm, 1, CipherMode.ElectronicCodebook, CipherTransformationMode.Encryption, null, block);
        
        Console.Write("Encryption result: ");
        for (var i = 0; i < block.Length; ++i)
        {
            Console.Write($"{block[i]} ");
        }
        Console.WriteLine();

        CryptoTransformationContext.PerformCipher(algorithm, 1, CipherMode.ElectronicCodebook, CipherTransformationMode.Decryption, null, block);
        
        Console.Write("Decryption result: ");
        for (var i = 0; i < block.Length; ++i)
        {
            Console.Write($"{block[i]} ");
        }
        Console.WriteLine();


    }
}