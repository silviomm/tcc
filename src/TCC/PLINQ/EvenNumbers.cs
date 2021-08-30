using System.Collections.Generic;
using System.Linq;
using TCC.Utils;

namespace TCC.PLINQ
{
    public class EvenNumbers
    {
        
        public static List<int> GetParallelList(int size)
        {
            var parallelSource = Enumerable.Range(0, size).AsParallel();
            return parallelSource.Where(x => x % 2 == 0).Select(e =>
            {
                Logger.Log($"Número par: {e}");
                return e;
            }).ToList();
        }
        
        public static List<int> GetSequentialList(int size)
        {
            var sequentialSource = Enumerable.Range(0, size);
            return sequentialSource.Where(x => x % 2 == 0).Select(e =>
            {
                Logger.Log($"Número par: {e}");
                return e;
            }).ToList();
        }
        
        public static ParallelQuery<int> GetParallelQuery(int size)
        {
            var parallelSource = Enumerable.Range(0, size).AsParallel();
            return parallelSource.Where(x => x % 2 == 0).Select(e =>
            {
                Logger.Log($"Número par: {e}");
                return e;
            });
        }
    }
}