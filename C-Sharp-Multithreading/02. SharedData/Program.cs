namespace _02._SharedData;

public class Program
{
    private static bool done;

    public static void Main()
    {
       // new Thread(Go).Start();
            
       // Go();

        CapturedVariables();
    }

    private static void Go()
    {
        if (!done)
        {
            Console.WriteLine("Done");
            done = true; // Put this statement before the Console writing to see a different result.
        }
    }

    private static void CapturedVariables()
    {
        for (int i = 0; i < 10; i++)
        {
            var current = i;
            new Thread(() =>
            {
                Thread.Sleep(3000);
                Console.Write(current);
            }).Start();
        }
    }
}