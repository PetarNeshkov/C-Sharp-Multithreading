namespace _15._ReaderWriterLock;

public class UpgradableReadLock
{
    static ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
    static List<int> items = new List<int>();
    static Random random = new Random();

    public static void Run()
    {
        new Thread(Read).Start(1);
        new Thread(Read).Start(2);
        new Thread(Read).Start(3);

  

        Console.ReadKey();
    }

    static void Read(object? threadId)
    {
        while (true)
        {
            locker.EnterUpgradeableReadLock();
            try
            {
                foreach (var item in items)
                {
                    Console.WriteLine("Thread " + threadId + " read " + item);
                    Thread.Sleep(100);
                }

                // Check a condition that may require upgrading to a write lock.
                if (SomeConditionToUpgrade())
                {
                    locker.EnterWriteLock();
                    try
                    {
                        // Perform write operations while holding the write lock.
                        var newNumber = GetRandNum(100);
                        items.Add(newNumber);
                        Console.WriteLine("Thread " + threadId + " added " + newNumber);
                    }
                    finally
                    {
                        locker.ExitWriteLock();
                    }
                }
            }
            finally
            {
                locker.ExitUpgradeableReadLock();
            }
        }
    }

    static void Write(object? threadId)
    {
        while (true)
        {
            locker.EnterWriteLock();
            try
            {
                // Perform write operations here.
                var newNumber = GetRandNum(100);
                items.Add(newNumber);
                Console.WriteLine("Thread " + threadId + " added " + newNumber);
            }
            finally
            {
                locker.ExitWriteLock();
            }

            Thread.Sleep(1000);
        }
    }

    static int GetRandNum(int max)
    {
        lock (random)
        {
            return random.Next(max);
        }
    }

    static bool SomeConditionToUpgrade()
    {
        // Replace this with your actual condition.
        return random.Next(1, 10) == 1;
    }
}