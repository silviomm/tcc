using System;
using System.Threading;

namespace TCC.Utils
{
    public static class Logger
    {
        public static void Log(string msg)
        {
            Console.WriteLine($"Time: {DateTime.Now.Minute}:{DateTime.Now.Second}:{DateTime.Now.Millisecond} - Thread Id: {Thread.CurrentThread.ManagedThreadId} - {msg}");
        }
    }
}