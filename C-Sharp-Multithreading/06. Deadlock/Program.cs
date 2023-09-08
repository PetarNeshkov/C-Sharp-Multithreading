object FirstLocker = new object();
object SecondLocker = new object();

var t1 = new Thread(DeadLock1);
var t2 = new Thread(DeadLock2);

t1.Start();
t2.Start();

void DeadLock1()
{
    Console.WriteLine("Thread 1 got locked");
    lock (SecondLocker)
    {
        Console.WriteLine("Thread 2 got locked");
    }
}

void DeadLock2()
{
    lock (FirstLocker)
    {
        Console.WriteLine("Thread 1 got locked");
        Thread.Sleep(500);

        lock (SecondLocker)
        {
            Console.WriteLine("Thread 2 got locked");
        }
    }
}