// Initial capacity of 3, maximum capacity of 3.

SemaphoreSlim locker = new SemaphoreSlim(3, 3);

for (int i = 1; i <= 10; i++)
{
    new Thread(Enter!).Start(i);
}

void Enter(object id)
{
    Console.WriteLine(id + " wants to enter");

    locker.Wait(); // -1
    
    Console.WriteLine(id + " is in!");
    
    Thread.Sleep(1000 * (int)id); // can be here at

    Console.WriteLine(id + " is leaving"); // a time.

    locker.Release(); // +1
}