using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TCC.PLINQ
{
    class AbMatrix
    {
        public int ARows { get; set; }
        public int BRows { get; set; }
        public int ACols { get; set; }
        public int BCols { get; set; }
    }
    
    public static class MatrixUtils
    {
        public static double[][] Init(int rows, int cols)
        {
            var result = new double[rows][];
            for (var i = 0; i < rows; ++i)
                result[i] = new double[cols];
            return result;
        }
        
        public static void Print(double[][] m)
        {
            for (var row = 0; row <= m.Length-1; row++)
            {
                Console.Write("[");
                for (var column = 0; column <= m[0].Length - 1; column++)
                {
                    Console.Write($"{m[row][column]}, ");
                }

                Console.WriteLine("]");
            }
        }

        public static async void Create(int n, int m, string path)
        {
            List<string> lines = new List<string>
            {
                $"{n} {m}"
            };
            
            Random r = new Random(1);
            for (var i = 0; i < n; i++)
            {
                var line = "";
                for (var j = 0; j < m; j++)
                {
                    var number = double.Parse(r.Next(100).ToString());
                    line = j == 0 ? $"{number}" : $"{line} {number}";
                }
                lines.Add(line);
            }

            await File.WriteAllLinesAsync(path, lines);
        }
        public static double[][] ReadMatrix(string matrixPath)
        {
            double[][] matrix = { };
            try
            {
                using StreamReader sr = new StreamReader(matrixPath);
                var size = sr.ReadLine()?.Split(" ");
                if (size != null) matrix = Init(int.Parse(size[0]), int.Parse(size[1]));

                var i = 0;
                while (!sr.EndOfStream)
                {
                    var row = sr.ReadLine()?.Split(" ");
                    if (row == null) throw new Exception("null row inside file");
                    
                    Array.Resize(ref row, row.Length-1);
                    var j = 0;
                    foreach (var n in row)
                    {
                        matrix[i][j] = double.Parse(n);
                        j++;
                    }
                    i++;
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return matrix;
        }

        private static AbMatrix CheckMultiplicity(double[][] a, double[][] b)
        {
            var aRows = a.Length;
            var aCols = a[0].Length;
            var bRows = b.Length;
            var bCols = b[0].Length;
            if (aCols != bRows)
                throw new Exception("cant multiply");
            return new AbMatrix{ARows = aRows, BRows = bRows, ACols = aCols, BCols = bCols};
        }
        
        public static double[][] Sequential(double[][] a, double[][] b)
        {
            AbMatrix abMatrix = CheckMultiplicity(a, b);
            var result = Init(abMatrix.ARows, abMatrix.BCols);

            a.Select((row, i) => new {row, i}).ToList().ForEach(objA =>
            {
                for (var j = 0; j < abMatrix.BCols; ++j)
                for (var k = 0; k < abMatrix.ACols; ++k)
                    result[objA.i][j] += a[objA.i][k] * b[k][j]; 
            });
            return result;
        }

        public static double[][] Parallel(double[][] a, double[][] b, int parallelism = 12)
        {
            AbMatrix abMatrix = CheckMultiplicity(a, b);
            var result = Init(abMatrix.ARows, abMatrix.BCols);

            a.AsParallel().WithDegreeOfParallelism(parallelism).Select((row, i) => new {row, i}).ForAll(objA =>
            {
                for (var j = 0; j < abMatrix.BCols; j++)
                for (var k = 0; k < abMatrix.ACols; k++)
                    result[objA.i][j] += a[objA.i][k] * b[k][j];
            });
            return result;
        }
        
        public static void Correctness()
        {
            double[][] lhs = {
                new[] {1.0, 2.0},
                new[] {3.0, 4.0}
            };
            
            double[][] rhs = {
                new[] {5.0, 6.0, 7.0},
                new[] {8.0, 9.0, 10.0}
            };
            
            Print(Sequential(lhs, rhs));
            Console.WriteLine("---------------------");
            Print(Parallel(lhs, rhs));
        }
    }
}