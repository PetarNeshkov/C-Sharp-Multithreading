namespace _08._Events;

public class Manual
{
    private static readonly ManualResetEventSlim WaitHandle = new ManualResetEventSlim(false);

    public static void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            var current = i;
            new Thread(Waiter).Start(current); 
        }
        
        Thread.Sleep(2000); // Pause for 2 seconds
        WaitHandle.Set(); // Wake up waiter (Release all threads) 
    }

    private static void Waiter(object id)
    {
        Console.WriteLine($"{id} waiting auto event...");
        WaitHandle.Wait(); //Wait for notification (blocks current thread)
        Console.WriteLine($"{id} notified and unblocked!");
    }
}