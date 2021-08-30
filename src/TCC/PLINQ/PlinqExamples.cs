using System;
using System.Linq;
using BenchmarkDotNet.Running;
using TCC.Utils;

namespace TCC.PLINQ
{
    public class PlinqExamples
    {
        
        public static void Benchmark()
        {
            BenchmarkRunner.Run<MatrixMultiplicationBenchmark>();
            BenchmarkRunner.Run<PlinqBenchmarkEvenNumbers>();
        }

        public static void SequentialAndParallelEvenNumbers()
        {
            Logger.Log("(Sequencial) Iniciando processamento de números pares:");
            EvenNumbers.GetSequentialList(10);
            Logger.Log("(Paralelo) Iniciando processamento de números pares:");
            EvenNumbers.GetParallelList(10);
        }
        
        public static void ForAll()
        {
            Logger.Log("(foreach) Iniciando processamento de números pares:");
            ParallelQuery<int> s = EvenNumbers.GetParallelQuery(10);
            foreach (var i in s)
            {
                Logger.Log($"Somando 1 sequencialmente: {i+1}");
            }
            
            Logger.Log("(ForAll) Iniciando processamento de números pares:");
            ParallelQuery<int> p = EvenNumbers.GetParallelQuery(10);
            p.ForAll(e =>
            {
                Logger.Log($"Somando 1 em paralelo: {e+1}");
            });
        }

    }
}