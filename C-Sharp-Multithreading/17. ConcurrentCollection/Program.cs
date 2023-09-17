using System.Collections.Concurrent;

Random random = new Random();
CountdownEvent countdown = new CountdownEvent(100);
ConcurrentDictionary<string, int> dictionary = new ConcurrentDictionary<string, int>();
ConcurrentBag<int> values = new ConcurrentBag<int>();

for (int i = 0; i < 100; i++)
{
    var current = i;

    var thread = new Thread(() => Work("Key", current));
    
    thread.Start();
}

countdown.Wait();

Parallel.ForEach(values, Console.WriteLine);

Console.WriteLine("-----");

Console.WriteLine(values.Count);

return;

void Work(string key, int value)
{
    Thread.Sleep(random.Next(100, 2000));
    
    var result = dictionary.GetOrAdd(key, _ => value);
    
    Thread.Sleep(random.Next(100, 2000));

    if (dictionary.TryRemove(key, out var newValue))
    {
        if (dictionary.ContainsKey(key))
        {
            Console.WriteLine("Still inside!");
        }
        
        values.Add(newValue);
    }

    countdown.Signal();
}