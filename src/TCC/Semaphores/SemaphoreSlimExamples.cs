using System;
using System.Threading;
using TCC.Utils;

namespace TCC.Semaphores
{
    public class SemaphoreSlimExamples
    {
        public static void WaitReleaseExample()
        {
            SemaphoreSlim local = new SemaphoreSlim(1, 1);
            Logger.Log("oie");
            for (int i = 0; i < 3; i++)
            {
                new Thread(async () =>
                {
                    Logger.Log($"esperando...");
                    await local.WaitAsync();
                    Logger.Log($"executando!");
                    Thread.Sleep(1000);
                    Logger.Log($"liberando...");
                    local.Release();
                }).Start();
            }

            Console.ReadLine();
        }
    }
}