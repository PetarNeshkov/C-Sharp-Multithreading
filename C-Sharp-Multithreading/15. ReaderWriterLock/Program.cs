using _15._ReaderWriterLock;

ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
List<int> items = new List<int>();
Random random = new Random();

//new Thread(Read).Start(1);
//new Thread(Read).Start(2);
//new Thread(Read).Start(3);
//
//new Thread(Write).Start("A");
//new Thread(Write).Start("B");

UpgradableReadLock.Run();

void Read(object? threadId)
{
    while (true)
    {
        try
        {
            locker.EnterReadLock();

            foreach (var item in items)
            {
                Console.WriteLine("Thread " + threadId + " read " + item);
                Thread.Sleep(100);
            }
        }
        finally
        {
            locker.ExitReadLock();
        }
    }
}

void Write(object? threadId)
{
    while (true)
    {
        var newNumber = GetRandNum(100);

        try
        {
            locker.EnterWriteLock();

            items.Add(newNumber);
        }
        finally
        {
            locker.ExitWriteLock();
        }


        Console.WriteLine("Thread " + threadId + " added " + newNumber);
        Thread.Sleep(100);
    }
}

int GetRandNum(int max)
{
    lock (random)
    {
        return random.Next(max);
    }
}