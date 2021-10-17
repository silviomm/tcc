using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TCC.Utils;

namespace TCC.CustomTaskSchedulers
{
    public class SimpleTaskScheduler : TaskScheduler
    {
        private readonly ConcurrentQueue<Task> _tasks = new ConcurrentQueue<Task>();

        public static void SimpleTaskSchedulerExample()
        {
            SimpleTaskScheduler scheduler = new SimpleTaskScheduler();
            for (int i = 0; i < 10; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    Logger.Log($"(Task) Executando tarefa {i}");
                }, CancellationToken.None, TaskCreationOptions.None, scheduler);   
            }
        }
        
        public SimpleTaskScheduler()
        {
            var periodTimeSpan = TimeSpan.FromSeconds(10);

            Logger.Log($"Criando timer para execução de tasks a cada {periodTimeSpan.Seconds}s...");

            new Thread(() =>
            {
                Console.WriteLine("Começando...");
                while (true)
                {
                    Thread.Sleep(10000);
                    ExecuteTask();
                }
            }).Start();
        }

        private void ExecuteTask()
        {
            if (_tasks.Count <= 0)
            {
                Logger.Log($"Sem tasks para executar");
            }
            else
            {   
                Logger.Log($"Tentando retirar task da fila. Qntd: {_tasks.Count}");
                if (_tasks.TryDequeue(out Task task))
                {
                    Logger.Log($"Retirada task {task.Id}");
                    TryExecuteTask(task);
                }
                else
                {
                    Logger.Log($"Falha ao retirar da fila");
                }
            }
        }
        
        protected override IEnumerable<Task>? GetScheduledTasks()
        {
            return _tasks.ToArray();
        }

        protected override void QueueTask(Task task)
        {
            Logger.Log($"Enfileirando task: {task.Id}");
            _tasks.Enqueue(task);
            Logger.Log($"Numero de tasks: {_tasks.Count}");
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }
    }
}