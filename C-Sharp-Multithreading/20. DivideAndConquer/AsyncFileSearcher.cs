namespace DivideAndConquer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public class AsyncFileSearcher
    {
        private readonly string fileContent;

        public AsyncFileSearcher(string filePath)
            => this.fileContent = File.ReadAllText(filePath);

        public async Task<int> Search(string searchTerm)
        {
            var totalProcessors = Environment.ProcessorCount;

            var totalLength = this.fileContent.Length;

            var partLength = (int)Math.Ceiling((double)totalLength / totalProcessors);

            var count = 0;

            var tasks = new List<Task>();

            for (var i = 0; i < totalProcessors; i++)
            {
                var current = i;

                var task = Task.Run(() =>
                {
                    var (startIndex, endIndex) = this.GetPartIndices(current, partLength);

                    var threadCount = 0;

                    var currentIndex = startIndex - 1;

                    while (true)
                    {
                        currentIndex = this.fileContent.IndexOf(
                            searchTerm,
                            currentIndex + 1,
                            endIndex - currentIndex - 1,
                            StringComparison.InvariantCulture);

                        if (currentIndex < 0)
                        {
                            break;
                        }

                        threadCount++;
                    }

                    Interlocked.Add(ref count, threadCount);
                });

                tasks.Add(task);
            }

            for (var i = 0; i < totalProcessors - 1; i++)
            {
                var (_, firstEndIndex) = this.GetPartIndices(i, partLength);
                var (secondStartIndex, _) = this.GetPartIndices(i + 1, partLength);

                var mergedStartIndex = firstEndIndex - (searchTerm.Length - 1);
                var mergedEndIndex = secondStartIndex + (searchTerm.Length - 1);

                var found = this.fileContent.IndexOf(
                    searchTerm,
                    mergedStartIndex,
                    mergedEndIndex - mergedStartIndex - 1,
                    StringComparison.InvariantCulture);

                if (found > -1)
                {
                    Interlocked.Increment(ref count);
                }
            }

            await Task.WhenAll(tasks);

            return count;
        }

        private (int StartIndex, int EndIndex) GetPartIndices(int part, int partLength)
        {
            var startIndex = part * partLength;
            var endIndex = (part + 1) * partLength;

            if (endIndex > this.fileContent.Length - 1)
            {
                endIndex = this.fileContent.Length;
            }

            return (startIndex, endIndex);
        }
    }
}
