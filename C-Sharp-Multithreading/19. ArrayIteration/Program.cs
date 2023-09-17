using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;

namespace _19._ArrayIteration;

public class Program
{
    private static int items = 200000000;
    private static int[] arr;

    public static void Main()
    {
        var program = new Program();

        using var process = Process.GetCurrentProcess();

        process.PriorityClass = ProcessPriorityClass.High;

        program.GenerateArray();

        var stopwatch = Stopwatch.StartNew();

        var regularResult = program.RegularIterationLocalArr();
        
        Console.WriteLine($"Regular iteration: {stopwatch.Elapsed}");     
        
        stopwatch = Stopwatch.StartNew();

        program.ThreadPoolWithLock();
        
        Console.WriteLine($"Thread pool with lock iteration: {stopwatch.Elapsed}");
        
        stopwatch = Stopwatch.StartNew();
        
        program.ThreadPoolWithInterLock();

        Console.WriteLine($"Thread pool with Interlock iteration: {stopwatch.Elapsed}");
        
        stopwatch = Stopwatch.StartNew();
        
        program.ParallelFor();
        
        Console.WriteLine($"Parallel.For iteration: {stopwatch.Elapsed}");
        
        stopwatch = Stopwatch.StartNew();
        
        program.ParallelForTemporaryTest();
        
        Console.WriteLine($"Parallel.For no Interlocked: {stopwatch.Elapsed}");
        
        stopwatch = Stopwatch.StartNew();
        
        program.ThreadPoolWithSmartLock();
        
        Console.WriteLine($"Thread pool with smart lock: {stopwatch.Elapsed}");
        
        stopwatch = Stopwatch.StartNew();
        
        
        var finalResult = program.ParallelForWithLocalFinally();

        Console.WriteLine($"Parallel.For with local variables: {stopwatch.Elapsed}");

        // var config = DefaultConfig.Instance
        //     .AddJob(Job.Default.WithToolchain(InProcessNoEmitToolchain.Instance));
        //
        // var benchmark = BenchmarkRunner.Run<Program>(config);
        // Console.WriteLine(benchmark);
        
        Console.WriteLine(regularResult == finalResult);
    }

    [Benchmark]
    public long ThreadPoolWithInterLock()
    {
        var total = 0L;
        var threads = 10;

        var countdown = new CountdownEvent(threads);

        var partSize = items / threads;

        for (var i = 0; i < threads; i++)
        {
            var localThread = i;

            ThreadPool.QueueUserWorkItem(_ =>
            {
                for (var j = localThread * partSize; j < (localThread + 1) * partSize; j++)
                {
                    Interlocked.Add(ref total, arr[j]);
                }

                countdown.Signal();
            });
        }

        countdown.Wait();

        return total;
    }

    [Benchmark]
    public long ParallelForWithLocalFinally()
    {
        var total = 0L;
        var parts = 10;
        var partSize = items / parts;

        var locker = new object();

        Parallel.For(0, parts,
            localInit: () => 0L,
            body: (i, state, localTotal) =>
            {
                for (int j = i * partSize; j < (i + 1) * partSize; j++)
                {
                    localTotal += arr[j];
                }

                return localTotal;
            },
            localFinally: localTotal =>
            {
                lock (locker)
                {
                    total += localTotal;
                }
            }
        );

        return total;
    }

    [Benchmark]
    public long ThreadPoolWithSmartLock()
    {
        var total = 0L;
        var threads = 10;

        var countdown = new CountdownEvent(threads);
        
        var partSize = items / threads;

        for (var i = 0; i < threads; i++)
        {
            var localThread = i;

            ThreadPool.QueueUserWorkItem(_ =>
            {
                var threadLocal = 0;

                for (var j = localThread * partSize; j < (localThread + 1) * partSize; j++)
                {
                    threadLocal += arr[j];
                }
                
                Interlocked.Add(ref total, threadLocal);

                countdown.Signal();
            });
        }

        countdown.Wait();

        return total;
    }

    [Benchmark]
    public long ParallelForTemporaryTest()
    {
        var total = 0L;
        var parts = 10;

        var partSize = items / parts;
        
        Parallel.For(0, parts, new ParallelOptions { MaxDegreeOfParallelism = parts }, i =>
        {
            for (var j = i * partSize; j < (i + 1) * partSize; j++)
            {
                var x = arr[j];
                // Interlocked.Add(ref total, arr[j]);
            }
        });

        return total;
    }

    [Benchmark]
    public long ParallelFor()
    {
        var total = 0;
        var parts = 10;

        var partSize = items / parts;

        Parallel.For(0, parts, new ParallelOptions { MaxDegreeOfParallelism = parts }, i =>
        {
            for (int j = i * partSize; j < (i + 1) * partSize; j++)
            {
                Interlocked.Add(ref total, arr[j]);
            }
        });

        return total;
    }

    [Benchmark]
    public long ThreadPoolWithLock()
    {
        var total = 0L;
        var threads = 10;

        var locker = new object();
        var countdown = new CountdownEvent(threads);

        var partSize = items / threads;

        for (int i = 0; i < threads; i++)
        {
            var localThread = i;
            
            ThreadPool.QueueUserWorkItem(_ =>
            {
                for (var j = localThread * partSize; j < (localThread + 1) * partSize; j++)
                {
                    lock (locker)
                    {
                        total += arr[j];
                    }
                }

                countdown.Signal();
            });
        }
        
        countdown.Wait();

        return total;
    }
    
    public long RegularIterationLocalArr()
    {
        var total = 0L;
        var threads = 10;


        var locker = new object();
        var countdown = new CountdownEvent(threads);
        
        var partSize = items / threads;

        for (int i = 0; i < threads; i++)
        {
            var localThread = i;

            ThreadPool.QueueUserWorkItem(_ =>
            {
                for (int j = localThread * partSize; j < (localThread + 1) * partSize; j++)
                {
                    lock (locker)
                    {
                        total += arr[j];
                    }
                }

                countdown.Signal();
            });
        }

        countdown.Wait();

        return total;
    }
    
    public void GenerateArray()
    {
        arr = new int[items];
        var rnd = new Random();

        for (int i = 0; i < items; i++)
        {
            arr[i] = rnd.Next(1000);
        }
    }
}