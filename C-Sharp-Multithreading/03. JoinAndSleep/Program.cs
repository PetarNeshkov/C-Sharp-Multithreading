var thread = new Thread(Go);

Console.WriteLine("Start");

thread.Start();

Console.WriteLine("Working");

thread.Join();

Console.WriteLine("Thread has ended!");

void Go()
{
    Thread.Sleep(1000);
    Console.WriteLine("Slept!");
}