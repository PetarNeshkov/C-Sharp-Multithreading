namespace FolderSync
{
    using System;
    using System.Diagnostics;

    public class Program
    {
        public static void Main()
        {
            const string source = "Source";
            const string destination = "Destination";

            using (var process = Process.GetCurrentProcess())
            {
                process.PriorityClass = ProcessPriorityClass.BelowNormal;
            }

            var folderSynchronizer = new BlockingCollectionFolderSynchronizer(source, destination);

            folderSynchronizer.Start();

            Console.WriteLine($"Folder '{destination}' is now synchronized with '{source}'.");
            Console.WriteLine("Enter 'stop' to end syncing...");

            while (true)
            {
                var line = Console.ReadLine();
                if (line?.ToLower() == "stop")
                {
                    folderSynchronizer.Stop();
                    break;
                }
            }
        }
    }
}