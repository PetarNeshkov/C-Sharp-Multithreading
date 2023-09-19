using System.Collections.Generic;
using System.Threading;

namespace DivideAndConquer
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class AsyncFileSearcher
    {
        private readonly string fileContent;

        public AsyncFileSearcher(string filePath)
            => this.fileContent = File.ReadAllText(filePath);

        public async Task<int> Search(string searchTerm)
        {
            var totalProcessors = Environment.ProcessorCount;

            var totalLength = fileContent.Length;

            var partLength = (int)Math.Ceiling((double)(totalLength) / totalProcessors);

            var totalCount = 0;

            var tasks = new List<Task>();

            for (int i = 0; i < totalProcessors; i++)
            {
                var currentProcessor = i;

                var task = Task.Run(() =>
                {
                    var (startIndex, endIndex) = GetPartIndices(currentProcessor, partLength);

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

                    Interlocked.Add(ref totalCount, threadCount);
                });

                tasks.Add(task);
            }

            for (int i = 0; i < totalProcessors - 1; i++)
            {
                var (_, firstEndIndex) = GetPartIndices(i, partLength);
                var (secondStartIndex, _) = GetPartIndices(i + 1, partLength);

                var mergedStartIndex = firstEndIndex - (searchTerm.Length - 1);
                var mergedEndIndex = secondStartIndex + (searchTerm.Length - 1);

                var found = fileContent.IndexOf(
                    searchTerm,
                    mergedStartIndex,
                    mergedEndIndex - mergedStartIndex - 1,
                    StringComparison.InvariantCulture);

                if (found > -1)
                {
                    Interlocked.Increment(ref totalCount);
                }
            }

            await Task.WhenAll(tasks);

            return totalCount;
        }

        private (int startIndex, int endIndex) GetPartIndices(int part, int partLength)
        {
            var startIndex = part * partLength;
            var endIndex = (part + 1) * partLength;

            if (endIndex > fileContent.Length - 1)
            {
                endIndex = fileContent.Length;
            }

            return (startIndex, endIndex);
        }
    }
}