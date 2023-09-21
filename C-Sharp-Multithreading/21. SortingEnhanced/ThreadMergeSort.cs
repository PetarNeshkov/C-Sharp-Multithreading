using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SortingEnhanced
{
    using System;

    public static class ThreadMergeSort
    {
        public static void ParallelSort(int[] input, int left, int right)
        {
            if (left < right)
            {
                var middle = (left + right) / 2;

                if (right - left < 20000)
                {
                    Sort(input, left, middle);
                    Sort(input, middle + 1, right);
                    
                    return;
                }
                
                // Slower variant
                
                // else
                // {
                //     Parallel.Invoke(
                //         () => ParallelSort(input, left, middle),
                //         () => ParallelSort(input, middle + 1, right));
                // }
                
                // Fallback to normal merge sort.
                if (input.Length < 20000)
                {
                    Sort(input, 0, input.Length - 1);
                    return;
                }

                var threads = 8;

                var partIndices = CalculatePartIndices(input, threads);

                SortThreadsParts(input, threads, partIndices);
                MergeFinalParts(input, partIndices);
            }
        }
        
        private static void SortThreadsParts(int[] input, int threads, List<int> partIndices)
        {
            var countdown = new CountdownEvent(threads);

            for (int i = 0; i < partIndices.Count - 1; i++)
            {
                var left = partIndices[i] + 1;
                var right = partIndices[i + 1];

                var thread = new Thread(() =>
                {
                    Sort(input, left, right);
                    countdown.Signal();
                });

                thread.Start();
            }

            countdown.Wait();
        }
        
        private static List<int> CalculatePartIndices(int[] input, int threads)
        {
            var partLength = (int)Math.Ceiling((double)input.Length / threads);
            var partIndices = new List<int> { -1 };

            var done = false;

            while (!done)
            {
                var next = partIndices[^1] + partLength;

                if (next >= input.Length - 1)
                {
                    next = input.Length - 1;
                    done = true;
                }

                partIndices.Add(next);
            }

            return partIndices;
        }
        
        private static void MergeFinalParts(int[] input, List<int> partIndices)
        {
            var increment = 1;
            var done = false;

            while (!done)
            {
                var count = 0;

                while (count != partIndices.Count - 1)
                {
                    var left = partIndices[count] + 1;
                    count += increment;
                    var middle = partIndices[count];
                    count += increment;
                    var right = partIndices[count];

                    Merge(input, left, middle, right);

                    if (left == 0 && right == input.Length - 1)
                    {
                        done = true;
                        break;
                    }
                }

                increment *= 2;
            }

            // Merge(input, partIndices[0] + 1, partIndices[1], partIndices[2]);
            // Merge(input, partIndices[2] + 1, partIndices[3], partIndices[4]);
            // Merge(input, partIndices[4] + 1, partIndices[5], partIndices[6]);
            // Merge(input, partIndices[6] + 1, partIndices[7], partIndices[8]);
               
            // Merge(input, partIndices[0] + 1, partIndices[2], partIndices[4]);
            // Merge(input, partIndices[4] + 1, partIndices[6], partIndices[8]);
               
            // Merge(input, partIndices[0] + 1, partIndices[4], partIndices[8]);
        }
        
        public static void Sort(int[] input, int left, int right)
        {
            if (left < right)
            {
                var middle = (left + right) / 2;

                Sort(input, left, middle);
                Sort(input, middle + 1, right);

                Merge(input, left, middle, right);
            }
        }

        private static void Merge(int[] input, int left, int middle, int right)
        {
            var leftArray = new int[middle - left + 1];
            var rightArray = new int[right - middle];

            Array.Copy(input, left, leftArray, 0, middle - left + 1);
            Array.Copy(input, middle + 1, rightArray, 0, right - middle);

            var i = 0;
            var j = 0;

            for (var k = left; k < right + 1; k++)
            {
                if (i == leftArray.Length)
                {
                    input[k] = rightArray[j];
                    j++;
                }
                else if (j == rightArray.Length)
                {
                    input[k] = leftArray[i];
                    i++;
                }
                else if (leftArray[i] <= rightArray[j])
                {
                    input[k] = leftArray[i];
                    i++;
                }
                else
                {
                    input[k] = rightArray[j];
                    j++;
                }
            }
        }
    }
}
