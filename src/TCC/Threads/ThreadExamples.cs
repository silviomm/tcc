using System.Threading;
using TCC.Utils;

namespace TCC.Threads
{
    public class ThreadExamples
    {
        public static int sharedCounter = 0;
        public static readonly object block = new object();
        
        public static void MainThreadExamples()
        {
            LockExample();
            SimpleThreadIsAliveExample();
        }

        private static void LockExample()
        {
            Thread t = new Thread(() => DoNothingLock("Trabalhadora"));
            t.Start();
            DoNothingLock("Principal");
            Logger.Log("Thread principal chamando Join()");
            t.Join();
            Logger.Log("Terminando LockExample");
        }
        
        private static void SimpleThreadIsAliveExample()
        {
            Thread t = new Thread(() => DoNothing("Trabalhadora"));
            Logger.Log($"Thread t \"isAlive\"? {t.IsAlive}");
            t.Start();
            Logger.Log($"Thread t \"isAlive\"? {t.IsAlive}");
            DoNothing("Principal");
            Logger.Log("Thread principal chamando Join()");
            t.Join();
            Logger.Log($"Thread t \"isAlive\"? {t.IsAlive}");
            Logger.Log("Terminando SimpleThreadIsAliveExample");
        }

        private static void DoNothing(string name, int sleepMili = 100)
        {
            for (int i = 0; i < 3; i++)
            {
                Logger.Log($"Thread \"{name}\" fazendo algo...");
                Thread.Sleep(sleepMili);
            }
        }

        private static void DoNothingLock(string name, int sleepMili = 100)
        {
            for (int i = 0; i < 3; i++)
            {
                lock (block)
                {
                    Logger.Log($"Thread \"{name}\" incrementando contador: {++sharedCounter}");
                }
                Thread.Sleep(sleepMili);
            }
        }
    }
}