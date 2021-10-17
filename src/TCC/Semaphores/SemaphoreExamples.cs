using System;
using System.Threading;
using TCC.Utils;

namespace TCC.Semaphores
{
    public class SemaphoreExamples
    {
        public static void WaitReleaseOverloadsExample()
        {
            Semaphore local = new Semaphore(1, 2);
            for (int i = 0; i < 3; i++)
            {
                var j = i;
                new Thread(() =>
                {
                    Logger.Log($"esperando até {j+1} segundos...");
                    if (local.WaitOne(TimeSpan.FromSeconds(j+1)))
                    {
                        Logger.Log($"executando seção critica!");
                        Thread.Sleep(2500);
                        Logger.Log($"liberando...");
                        local.Release(2);
                    }
                    else
                    {
                        Logger.Log($"Não foi possível entrar na seção crítica...");
                    }
                }).Start();
            }
        }
        
        public static void WaitReleaseExample()
        {
            Semaphore local = new Semaphore(1, 1);
            for (int i = 0; i < 3; i++)
            {
                new Thread(() =>
                {
                    Logger.Log($"esperando...");
                    local.WaitOne();
                    Logger.Log($"executando!");
                    Thread.Sleep(1000);
                    Logger.Log($"liberando...");
                    local.Release();
                }).Start();
            }
        }
        
        public static void OverloadsExample()
        {
            // Cria semáforo local, iniciado com valor 0 e máximo de 2
            Semaphore local = new Semaphore(2, 2);
            
            // Ambos os semáforos seguintes lançam exceção do tipo System.PlatformNotSupportedException ao rodar em Unix

            // Cria semáforo global de nome "global1", com valor inicial 1 e máximo 3
            Semaphore global1 = new Semaphore(1, 3, "global1");
            bool created;
            // Cria semáforo global de nome "global2", com valor inicial 0 e máximo 3, e retorna na variavel "created" se o semáforo foi criado no sistema
            Semaphore global2 = new Semaphore(2, 3, "global2", out created);
        }
    }
}