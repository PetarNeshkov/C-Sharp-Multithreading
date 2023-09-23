using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileDownloading
{
    using System;
    using System.Net;

    public class AsyncFileDownloader
    {
        private readonly string[] urls;
        private readonly int maxThreads;
        private readonly SemaphoreSlim locker;
        private readonly CountdownEvent coutdown;
        private readonly ConcurrentDictionary<int, int> downloadPercentage;
        private readonly ConcurrentDictionary<int, long> downloadProgress;
       

        private long totalSize;
        private Timer timer;
        public AsyncFileDownloader(string[] urls, int maxThreads = 3)
        {
            this.urls = urls;
            this.maxThreads = maxThreads;
            locker = new SemaphoreSlim(maxThreads);
            coutdown = new CountdownEvent(urls.Length);
            downloadPercentage = new ConcurrentDictionary<int, int>();
            downloadProgress = new ConcurrentDictionary<int, long>();
        }

        public void Download()
        {
            Console.WriteLine("Starting download...");

            var localLocker = new object();

            Parallel.ForEach(
                urls,
                new ParallelOptions { MaxDegreeOfParallelism = maxThreads },
                localInit: () => 0L,
                body: (url, state, localTotal) => localTotal + GetFileSize(url),
                localFinally: (localTotal) => {
                    lock (localLocker)
                    {
                        totalSize += localTotal;
                    }
                });

            for (int i = 0; i < urls.Length; i++)
            {
                var url = urls[i];
                var fileOrder = i +1 ;

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                    DownloadFile(url, fileOrder);
                });
            }

            StartReporting();

            coutdown.Wait();
            timer.Dispose();

            ReportResults();
        }

        private void DownloadFile(string url, int fileOrder)
        {
            locker.Wait();

            try
            {
                var webClient = new WebClient();

                webClient.DownloadProgressChanged += (obj, progress) =>
                {
                    downloadPercentage[fileOrder] = progress.ProgressPercentage;
                    downloadProgress[fileOrder] = progress.BytesReceived;
                };

                webClient.DownloadFileCompleted += (obj, data) =>
                {
                    locker.Release();
                    coutdown.Signal();
                };

                webClient.DownloadFileAsync(new Uri(url), fileOrder.ToString());
            }
            catch
            {
                locker.Release();
                coutdown.Signal();
            }
           
        }

        private long GetFileSize(string url)
        {
            var webClient = new WebClient();

            using var readStream = webClient.OpenRead(url);

            return long.Parse(webClient.ResponseHeaders["Content-Length"]);
        }

        private void StartReporting()
        {
            timer = new Timer(_ =>
            {
                ReportResults();
            }, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1));
        }

        private void ReportResults()
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(0,0);

            var totalDownloaded = downloadProgress.Values.Sum();
            var percentage = (double)totalDownloaded / totalSize * 100;

            var totalDownloadedInMb = totalDownloaded / 1024 / 1024;
            var totalSizeInMb = totalSize / 1024 / 1024;
            
            Console.Write($"Progress - {totalDownloadedInMb}/{totalSizeInMb} MB - {percentage:F2}%");

            foreach (var (key,value) in downloadPercentage)
            {
                Console.SetCursorPosition(0, key + 1);
                
                Console.Write($"{key} - {value}%");
            }
        }
    }
}
