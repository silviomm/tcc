using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TCC.Helpers;
using TCC.Utils;

namespace TCC.CustomTaskSchedulers
{
    public class PriorityTaskSchedulerManager
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly PriorityQueue<PriorityTask> _taskQueue = new PriorityQueue<PriorityTask>();
        private readonly Dictionary<int, PriorityTaskSchedulerWorker> _schedulers = new Dictionary<int, PriorityTaskSchedulerWorker>();

        public PriorityTaskSchedulerManager(int concurrencyLevel = -1)
        {
            int c = concurrencyLevel <= 0 ? Environment.ProcessorCount : concurrencyLevel;
            Logger.Log($"Criando {c} consumidores");
            
            // Cria threads consumidoras
            for (int i = 0; i < c; i++)
            {
                new Thread(ConsumerThread).Start();
            }
        }

        public void EnqueuePriority(PriorityTask pt)
        {
            _semaphore.Wait();
            Logger.Log($"Enfileirando tarefa de Id: {pt.Id} e Prioridade: {pt.Priority}");
            _taskQueue.Enqueue(pt);
            _semaphore.Release();
        }
        
        private void ConsumerThread()
        {
            while (true)
            {
                _semaphore.Wait();
                var success = _taskQueue.TryDequeue(out PriorityTask pt);
                _semaphore.Release();
                if (success)
                {
                    if(_schedulers.TryGetValue(pt.Priority, out PriorityTaskSchedulerWorker sch))
                    {
                        Logger.Log($"(Consumidora) Começando a executar tarefa de Id {pt.Id} e prioridade: {pt.Priority}");
                        sch.ExecuteTask(pt.Task);
                    }
                }
                else
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    Logger.Log("Dormindo 1s");
                }
            }
        }
        
        public bool AddScheduler(PriorityTaskSchedulerWorker scheduler)
        {
            try
            {
                _schedulers.Add(scheduler.Priority, scheduler);
                return true;
            }
            catch
            {
                Logger.Log($"Escalonador de prioridade {scheduler.Priority} já existe neste gerente");
                return false;
            }
        }
    }

    public class PriorityTask : IComparable<PriorityTask>
    {
        public int Id { get; set; }
        public Task Task { get; set; }
        public int Priority { get; set; }
        private DateTime CreationTime { get; set; }

        public PriorityTask(int id, int priority, Task task)
        {
            Id = id;
            CreationTime = DateTime.Now;
            Priority = priority;
            Task = task;
        }
        public int CompareTo(PriorityTask other)
        {
            if (Priority == other.Priority)
                return CreationTime.CompareTo(other.CreationTime);
            return Priority - other.Priority;
        }
    }
    
    public class PriorityTaskSchedulerWorker : TaskScheduler
    {
        private readonly PriorityTaskSchedulerManager _manager;
        public int Priority { get; set; }
        private static int _taskCounter = 0;
        private readonly ConcurrentQueue<Task> _tasks = new ConcurrentQueue<Task>();

        public PriorityTaskSchedulerWorker(int priority, PriorityTaskSchedulerManager manager)
        {
            _manager = manager;

            Priority = priority;
            if (!manager.AddScheduler(this))
            {
                throw new Exception("Could not create priorityTaskScheduler");
            }
        }

        public bool ExecuteTask(Task t)
        {
            _tasks.TryDequeue(out var deq);
            return TryExecuteTask(t);
        }

        protected override IEnumerable<Task>? GetScheduledTasks()
        {
            return _tasks;
        }

        protected override void QueueTask(Task task)
        {
            _tasks.Enqueue(task);
            _manager.EnqueuePriority(new PriorityTask(Interlocked.Increment(ref _taskCounter), Priority, task));
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }
    }
}