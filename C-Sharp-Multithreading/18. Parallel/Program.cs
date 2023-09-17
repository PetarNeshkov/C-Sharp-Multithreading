using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

public class Program
{
    public static void Main()
    {
        var program = new Program();
        
        // Invoke multiple jobs.
        Parallel.Invoke(
            () => new WebClient().DownloadFile("http://google.com", "google.html"),
            () => new WebClient().DownloadFile("http://facebook.com", "facebook.html"));

        // Invoke multiple jobs and save result.
        var bag = new ConcurrentBag<string>(); //Use a thread-safe collection.

        Parallel.Invoke(
            new ParallelOptions { MaxDegreeOfParallelism = 6 },
            () => bag.Add(new WebClient().DownloadString("http://google.com")),
            () => bag.Add(new WebClient().DownloadString("http://facebook.com")));

        // For loop.
        Parallel.For(0, 100, index => Console.Write($"{index} "));

        Console.WriteLine();

        // ForEach loop.
        Parallel.ForEach("Parallel!", (c, state) =>
        {
            if (c == '!')
            {
                state.Break();
            }
            else
            {
                Console.WriteLine(c);
            }
        });

        // Calculations with local thread-safe values.

        var stopwatch = Stopwatch.StartNew();
        // Bad example - too many locks.
        program.BadThreadExecutionPractice();

        Console.WriteLine($"Bad Practice: {stopwatch.Elapsed}");

        stopwatch = Stopwatch.StartNew();
        // Good example - less locks.
        program.GoodThreadExecutionPractice();

        Console.WriteLine($"Good Practice: {stopwatch.Elapsed}");

        var benchmark = BenchmarkRunner.Run<Program>();
        Console.WriteLine(benchmark);

    }
    
    [Benchmark]
    public void BadThreadExecutionPractice()
    {
        var badLocker = new object();
        var badTotal = 0.0;
        Parallel.For(1, 10000000,
            i =>
            {
                lock (badLocker)
                {
                    badTotal += Math.Sqrt(i);
                }
            });

        Console.WriteLine(badTotal);
    }
    
    [Benchmark]
    public void GoodThreadExecutionPractice()
    {
        var locker = new object();
        var total = 0.0;

        Parallel.For(1, 10000000,
            () => 0.0,                       // Initialize the local value.
            
            (i, state, localTotal) =>       // Body delegate. Notice that it
                localTotal + Math.Sqrt(i),          // returns the new local total.
            
            localTotal =>
            {
                lock (locker)
                {
                    total += localTotal;
                }
            }
            
        );

        Console.WriteLine(total);
    }
}
