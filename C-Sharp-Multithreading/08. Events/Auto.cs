namespace _08._Events;

public class Auto
{
    private static readonly EventWaitHandle WaitHandle = new AutoResetEvent(false);

    public static void Start()
    {
        new Thread(Waiter).Start();
        Thread.Sleep(2000); // Pause for 2 seconds
        WaitHandle.Set(); // Wake up the waiter (Release the thread)
    }

    private static void Waiter()
    {
        Console.WriteLine("Waiting auto event...");
        WaitHandle.WaitOne(); //Wait for notification (blocks current thread)
        Console.WriteLine("Notified and unblocked");
    }
}