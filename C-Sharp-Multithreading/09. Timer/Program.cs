Timer timer = new Timer(DoWork, null, 5000, 1000);

Console.ReadLine();

timer.Dispose(); // Stops the timer and cleans up.

void DoWork(object? work)
{
  Console.WriteLine("Repetitive work executed at: " + DateTime.Now);
}