namespace _12._ConsumerProducerPattern;

public class ConsumerProducerQueue : IDisposable
{
    private readonly EventWaitHandle waitHandle = new AutoResetEvent(false);
    private readonly object locker = new();
    private readonly Queue<string?> tasks = new();

    private readonly Thread worker;

    public ConsumerProducerQueue()
    {
        worker = new Thread(Work);
        worker.Start();
    }

    public void EnqueueTask(string? task)
    {
        lock (locker)
        {
            tasks.Enqueue(task);
        }

        waitHandle.Set();
    }

    public void Dispose()
    {
        EnqueueTask(null);  // Signal the consumer to exit.
        worker.Join();      // Wait for the consumer's thread to finish.
        waitHandle.Close(); // Release any OS resources.
    }

    private void Work()
    {
        while (true)
        {
            string? task = null;
            lock (locker)
            {
                if (tasks.Count > 0)
                {
                    task = tasks.Dequeue();
                    if (task == null)
                    {
                        return;
                    }
                }
            }

            if (task != null)
            {
                Console.WriteLine("Performing task: " + task);
                Thread.Sleep(1000); // simulate work...
            }
            else
            {
                waitHandle.WaitOne(); // No more tasks - wait for a signal.
            }
        }
    }
}
