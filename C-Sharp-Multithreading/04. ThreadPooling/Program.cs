//ThreadPoolWork();

//AsyncDelegates();

TaskBasedWork();


return;

void TaskBasedWork()
{
    var task = Task.Run(() =>
    {
        Go(123);
    });

    task.Wait();
}

void AsyncDelegates()
{
    Func<string, int> method = Result;
    var asyncResult = method.BeginInvoke("Test", null, null);

    Console.WriteLine("In between");

    var result = method.EndInvoke(asyncResult);

    Console.WriteLine(result);
}

int Result(string text)
{
    Thread.Sleep(1000);
    return text.Length;
}

void ThreadPoolWork()
{
    //equivalent of Task.Run(() => Go);
    ThreadPool.QueueUserWorkItem(Go, 123);
    
}

void Go(object? data)
{
    Console.WriteLine("From the thread pool! "+ data);

    Console.WriteLine(Thread.CurrentThread.IsThreadPoolThread);
}