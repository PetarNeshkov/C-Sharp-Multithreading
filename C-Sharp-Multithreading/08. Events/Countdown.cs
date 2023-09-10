namespace _08._Events;

public class Countdown
{
    private static readonly Random Random = new Random();
    private static readonly CountdownEvent CountdownEvent = new(3);

    public static void Start()
    {
        new Thread(SaySomething).Start("I am thread 1");
        new Thread(SaySomething).Start("I am thread 2");
        new Thread(SaySomething).Start("I am thread 3");

        CountdownEvent.Wait(); // Blocks until Signal has been called 3 times

        Console.WriteLine("All threads have finished speaking");
    }

    private static void SaySomething(object? obj)
    {
        Thread.Sleep(Random.Next(1000, 3000));
        Console.WriteLine(obj);
        
        CountdownEvent.Signal();
    }
}