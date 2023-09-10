using System.ComponentModel;

BackgroundWorker worker = new BackgroundWorker()
{
    WorkerReportsProgress = true,
    WorkerSupportsCancellation = true
};

worker.DoWork += OnDoWork;
worker.ProgressChanged += OnProgressChanged;
worker.RunWorkerCompleted += OnRunWorkerCompleted;

worker.RunWorkerAsync("Hello to worker");

Console.WriteLine("Press Enter in the next 5 seconds to cancel...");
Console.ReadLine();

if (worker.IsBusy)
{
    worker.CancelAsync();
}

Console.ReadLine();

void OnRunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
{
    if (e.Cancelled)
    {
        Console.WriteLine("You canceled!");
    }
    else if (e.Error != null)
    {
        Console.WriteLine("Worker exception: " + e.Error);
    }
    else
    {
        Console.WriteLine("Complete: " + e.Result);  // from OnDoWork
    }
}

void OnProgressChanged(object? sender, ProgressChangedEventArgs e)
{
    Console.WriteLine("Reached " + e.ProgressPercentage + "%");
}

void OnDoWork(object? sender, DoWorkEventArgs e)
{
    for (int i = 0; i <= 100; i+=20)
    {
        if (worker.CancellationPending)
        {
            e.Cancel = true;
            return;
        }
        
        worker.ReportProgress(i);
        Thread.Sleep(1000); // We only do sleeping for demo purposes! We don't do it in env.
    }

    e.Result = 123; // This gets passed to OnRunWorkerCompleted
}