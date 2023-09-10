Barrier barrier = new Barrier(3, _ => Console.WriteLine());

new Thread(Speak).Start();
new Thread(Speak).Start();
new Thread(Speak).Start();

void Speak()
{
    for (int i = 0; i < 5; i++)
    {
        Console.Write(i + " ");
        Thread.Sleep(1000);
        barrier.SignalAndWait();
    }
}