List<int> data = Enumerable.Range(0, 100).ToList();

object locker = new object();

for (int i = 0; i < 10; i++)
{
    var thread = new Thread(Work);
    thread.Start();
}

Console.WriteLine(data.Count);

return;
void Work()
{
    for (int i = 0; i < 5; i++)
    {
        Console.WriteLine(i);
        
        
        Thread.Sleep(500);

        lock (locker)
        {
            if (data.Count > 50)
            {
                data.RemoveAt(data.Count -1);
            }
        }
    }
}