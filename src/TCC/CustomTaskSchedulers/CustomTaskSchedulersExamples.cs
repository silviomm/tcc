using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TCC.Utils;

namespace TCC.CustomTaskSchedulers
{
    public class CustomTaskSchedulersExamples
    {
        
        public static async void MainCustomTaskSchedulersExamples() {
            SimpleTaskScheduler.SimpleTaskSchedulerExample();
            PrioritySimpleExample();
            TestNotPriorityOrdering();
            Console.ReadLine();
        }

        public static async void PrioritySimpleExample()
        {
            PriorityTaskSchedulerManager manager = new PriorityTaskSchedulerManager(1);
            PriorityTaskSchedulerWorker worker1 = new PriorityTaskSchedulerWorker(1, manager);
            PriorityTaskSchedulerWorker worker2 = new PriorityTaskSchedulerWorker(2, manager);

            for (int i = 0; i < 3; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    Logger.Log($"(Task) Executando tarefa de prioridade: {worker2.Priority}");
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    Logger.Log($"(Task) Terminada tarefa de prioridade: {worker2.Priority}");
                }, CancellationToken.None, TaskCreationOptions.None, worker2);
            }
            Thread.Sleep(TimeSpan.FromSeconds(3));
            Task.Factory.StartNew(() =>
            {
                Logger.Log($"(Task) Executando tarefa de prioridade: {worker1.Priority}");
                Thread.Sleep(TimeSpan.FromSeconds(5));
                Logger.Log($"(Task) Terminada tarefa de prioridade: {worker1.Priority}");
            }, CancellationToken.None, TaskCreationOptions.None, worker1);
        }

        public static async void PerformanceTestPriorityScheduler(int numberOfTasks)
        {
            PriorityTaskSchedulerManager manager = new PriorityTaskSchedulerManager();
            PriorityTaskSchedulerWorker worker1 = new PriorityTaskSchedulerWorker(1, manager);
            Stopwatch sw = new Stopwatch();
            List<Task> tasks = new List<Task>();
            sw.Start();
            for (int i = 0; i < numberOfTasks; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    // Logger.Log($"(Task) Executing task, priority {worker3.Priority}");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    // Logger.Log($"(Task) Finished Executing task, priority {worker3.Priority}");
                }, CancellationToken.None, TaskCreationOptions.None, worker1));
            }
            await Task.WhenAll(tasks);
            sw.Stop();
            Logger.Log($"PerformanceTestPriorityScheduler elapsed: {sw.Elapsed.TotalSeconds}");
        }
        
        public static async void PerformanceTestDefaultScheduler(int numberOfTasks)
        {
            Console.WriteLine(ThreadPool.SetMaxThreads(12, 12));
            Stopwatch sw = new Stopwatch();
            List<Task> tasks = new List<Task>();
            sw.Start();
            for (int i = 0; i < numberOfTasks; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    // Logger.Log($"(Task) Executing task, priority {worker3.Priority}");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    // Logger.Log($"(Task) Finished Executing task, priority {worker3.Priority}");
                }));
            }
            await Task.WhenAll(tasks);
            sw.Stop();
            Logger.Log($"PerformanceTestDefaultScheduler elapsed: {sw.Elapsed.TotalSeconds}");
        }

        public static async void TestNotPriorityOrdering()
        {
            ThreadPool.SetMaxThreads(1, 1);
            for (int i = 0; i < 3; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    Logger.Log($"(Task) Executando tarefa de prioridade: {2}");
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    Logger.Log($"(Task) Terminada tarefa de prioridade: {2}");
                });
            }
            await Task.Delay(TimeSpan.FromSeconds(3));
            Task.Factory.StartNew(() =>
            {
                Logger.Log($"(Task) Executando tarefa de prioridade: {1}");
                Thread.Sleep(TimeSpan.FromSeconds(5));
                Logger.Log($"(Task) Terminada tarefa de prioridade: {1}");
            });
        }
    }
}