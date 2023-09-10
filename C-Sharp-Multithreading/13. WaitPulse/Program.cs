object locker = new();
bool go = false;

// The new thread will block
new Thread(Work).Start(); // because _go==false.

Console.ReadLine(); // Wait for user to hit Enter


lock (locker)
{
    go = true; // Let's now wake up the thread by
    // setting _go=true and pulsing.
    Monitor.Pulse(locker);
}

void Work()
{
    lock (locker)
    {
        while (!go)
        {
            Monitor.Wait(locker); // Lock is released while we’re waiting
        }
    }

    Console.WriteLine("Woken!!!");
}