using System.Collections.Concurrent;

namespace _17._ConcurrentCollection;

public class TaskBasedConsumerQueue
{
    private readonly BlockingCollection<WorkItem> jobs = new();

    public TaskBasedConsumerQueue()
    {
        for (var i = 0; i < Environment.ProcessorCount / 2; i++)
        {
            ThreadPool.QueueUserWorkItem(_ => Work());
        }
    }

    public Task EnqueueTask(Action action) => EnqueueTask(action, null);

    public Task EnqueueTask(Action action, CancellationToken? cancellationToken)
    {
        var tcs = new TaskCompletionSource<object>();

        if (!jobs.IsAddingCompleted)
        {
            jobs.Add(new WorkItem(tcs!, action, cancellationToken));
        }
        else
        {
            return Task.FromException(new InvalidOperationException("Job queue is no longer operational."));
        }

        return tcs.Task;
    }

    public void Dispose() => jobs.CompleteAdding();

    private void Work()
    {
        foreach (var workItem in jobs.GetConsumingEnumerable()) // Blocks if empty!
        {
            if (workItem.CancellationToken.HasValue &&
                workItem.CancellationToken.Value.IsCancellationRequested)
            {
                workItem.TaskSource.SetCanceled();
            }
            else
            {
                try
                {
                    workItem.Action();
                    workItem.TaskSource.SetResult(null);   // Indicate completion.
                }
                catch (OperationCanceledException ex)
                {
                    if (ex.CancellationToken == workItem.CancellationToken)
                    {
                        workItem.TaskSource.SetCanceled();
                    }
                    else
                    {
                        workItem.TaskSource.SetException(ex);
                    }
                }
                catch (Exception ex)
                {
                    workItem.TaskSource.SetException(ex);
                }
            }
        }
    }
}