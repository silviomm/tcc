using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

namespace TCC.PLINQ
{
    public class PlinqBenchmarkConfig : ManualConfig
    {
        public PlinqBenchmarkConfig()
        {
            AddColumn(StatisticColumn.P95);
        }
    }

    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    [IterationCount(50)]
    [Config(typeof(PlinqBenchmarkConfig))]
    [SimpleJob(id: "MatrixMultiplication")]
    public class MatrixMultiplicationBenchmark
    {
        // [Params(100, 1000, 2000, 3000)]
        [Params(100)]
        public int matrix;

        private double[][] _a;
        private double[][] _b;

        [GlobalSetup]
        public void PlinqBenchmarkSetup()
        {
            var path = $"/home/silviomm/Documents/Projects/tcc/utils/{matrix}x{matrix}.txt";
            _a = MatrixUtils.ReadMatrix(path);
            _b = MatrixUtils.ReadMatrix(path);
        }
 
        [Benchmark(Baseline = true)]
        public double[][] Sequential()
        {
            return MatrixUtils.Sequential(_a, _b);
        }
        
        [Benchmark]
        public double[][] Parallel_2()
        {
            return MatrixUtils.Parallel(_a, _b, 2);
        }
        
        [Benchmark]
        public double[][] Parallel_6()
        {
            return MatrixUtils.Parallel(_a, _b, 6);
        }
        
        [Benchmark]
        public double[][] Parallel_12()
        {
            return MatrixUtils.Parallel(_a, _b, 12);
        }
    }
    
    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    [IterationCount(50)]
    [Config(typeof(PlinqBenchmarkConfig))]
    [SimpleJob(id: "PlinqBenchmarkEvenNumbers")]
    public class PlinqBenchmarkEvenNumbers
    {
        private int _range = 100;

        [GlobalSetup]
        public void PlinqBenchmarkSetup()
        {
        }
 
        [Benchmark(Baseline = true)]
        public List<int> Sequential()
        {
            return EvenNumbers.GetSequentialList(_range);
        }

        [Benchmark]
        public List<int> Parallel()
        {
            return EvenNumbers.GetParallelList(_range);
        }
    }
}