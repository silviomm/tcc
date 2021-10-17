using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace TCC.ProducerConsumer
{
    public class ProducerConsumer
    {
        private static TimeSpan TotalWaitTime = TimeSpan.FromMinutes(1);
        private static int bulkSize = 50;
        private static BlockingCollection<string> buffer = new BlockingCollection<string>();

        private static void BulkExample(CancellationToken token)
        {
            List<string> consumed = new List<string>();
            while (token.IsCancellationRequested)
            {
                TimeSpan waitTime = TotalWaitTime;
                DateTime startTime = DateTime.Now;

                while (consumed.Count < bulkSize && buffer.TryTake(out var item, waitTime))
                {
                    waitTime = TotalWaitTime - (DateTime.Now - startTime);
                    consumed.Add(item);
                }

                if (consumed.Count > 0)
                {
                    // insert in bulk
                    consumed.Clear();
                }
            }
        }
    }
}