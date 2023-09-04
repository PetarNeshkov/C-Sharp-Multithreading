MultipleThreads();

void MultipleThreads()
{
    var thread = new Thread(() =>
    {
        for (int i = 0; i < 1000; i++)
        {
            Console.Write("y");
        }
    });
    
    thread.Start();
    
    // Simultaneously, do something on the main thread.
    for (int i = 0; i < 1000; i++)
    {
        Console.Write("x"); 
    }
}