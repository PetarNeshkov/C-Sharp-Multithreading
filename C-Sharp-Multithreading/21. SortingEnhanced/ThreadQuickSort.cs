namespace SortingEnhanced
{
    using System.Threading;
    using System.Threading.Tasks;

    public static class ThreadQuickSort
    {
        public static void ParallelSort(int[] input, int low, int high)
        {
            // Fallback to normal quick sort.
            if (high - low < 20000)
            {
                Sort(input, low, high);
            }
            else
            {
                var part = Partition(input, low, high);

                //Slower variant
                
                // Parallel.Invoke(
                //     () => ParallelSort(input, low, part - 1),
                //     () => ParallelSort(input, part + 1, high));

                var countdown = new CountdownEvent(2);

                var leftThread = new Thread(() =>
                {
                    ParallelSort(input, low, part - 1);
                    countdown.Signal();
                });

                var rightThread = new Thread(() =>
                {
                    ParallelSort(input, part + 1, high);
                    countdown.Signal();
                });

                leftThread.Start();
                rightThread.Start();

                countdown.Wait();
            }
        }

        public static void Sort(int[] input, int low, int high)
        {
            if (high - low < 40)
            {
                InsertionSort(input, low, high);
            }
            else
            {
                var part = Partition(input, low, high);
                Sort(input, low, part - 1);
                Sort(input, part + 1, high);
            }
        }

        private static int Partition(int[] input, int low, int high)
        {
            var j = low;
            var pivot = input[low];

            for (int i = low; i <= high; i++)
            {
                if (input[i].CompareTo(pivot) >= 0)
                {
                    continue;
                }

                j++;
                Swap(input, i, j);
            }

            Swap(input, low, j);
            return j;
        }

        private static void InsertionSort(int[] input, int low, int high)
        {
            for (var i = low + 1; i <= high; i++)
            {
                var j = i - 1;
                var x = input[i];

                while (j >= 0 && input[j].CompareTo(x) > 0)
                {
                    input[j + 1] = input[j];
                    j--;
                }

                input[j + 1] = x;
            }
        }

        private static void Swap(int[] input, int i, int j)
        {
            (input[i], input[j]) = (input[j], input[i]);
        }
    }
}
