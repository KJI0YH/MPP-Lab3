using System.Collections.Concurrent;

namespace Core.Services
{
    public class TaskQueue
    {
        private ConcurrentQueue<Task> _queueWork;
        public SemaphoreSlim _semaphore;
        private CancellationTokenSource _tokerSource;
        private ConcurrentQueue<Task> _queueWait;
        public Task WaitTask;
        public Task WorkTask;

        public TaskQueue(int maxThreadCount, CancellationTokenSource cts)
        {
            _queueWork = new ConcurrentQueue<Task>();
            _queueWait = new ConcurrentQueue<Task>();
            _semaphore = new SemaphoreSlim(maxThreadCount);
            _tokerSource = cts;
            WorkTask = new Task(() =>
            {
                StartAllTask(_tokerSource.Token);
            }, _tokerSource.Token);
            WaitTask = new Task(() =>
            {
                WaitAllTask(_tokerSource.Token);
            }, _tokerSource.Token);
        }

        public void Add(Task item)
        {
            _queueWait.Enqueue(item);
            _queueWork.Enqueue(item);
        }

        private void WaitAllTask(CancellationToken token)
        {
            while (!_queueWait.IsEmpty && !token.IsCancellationRequested)
            {
                Task? task;
                bool result = _queueWait.TryDequeue(out task);
                if (result && task != null)
                {
                    try
                    {
                        task.Wait(token);
                        _semaphore.Release();
                    }
                    catch (TaskCanceledException)
                    {
                        return;
                    }
                }
            }
        }

        private void StartAllTask(CancellationToken token)
        {
            while (WaitTask.Status != TaskStatus.RanToCompletion && !token.IsCancellationRequested)
            {
                Task? task;
                bool result = _queueWork.TryDequeue(out task);
                if (result && task != null)
                {
                    try
                    {
                        _semaphore.Wait(token);
                        task.Start();
                    }
                    catch (TaskCanceledException)
                    {
                        return;
                    }
                }
            }
        }
    }
}
