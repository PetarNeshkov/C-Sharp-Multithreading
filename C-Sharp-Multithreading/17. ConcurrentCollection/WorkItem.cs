namespace _17._ConcurrentCollection;

public class WorkItem
{
    public WorkItem(TaskCompletionSource<object?> taskSource, Action action, CancellationToken? cancellationToken)
    {
        TaskSource = taskSource;
        Action = action;
        CancellationToken = cancellationToken;
    }


    public TaskCompletionSource<object?> TaskSource { get; set; }

    public Action Action { get; set; }

    public CancellationToken? CancellationToken { get; }
}