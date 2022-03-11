using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pl_compulsory_assignment
{
    internal class PrimeGenerator
    {
        private static object objLock = new object();
        public static List<Task> tasks = new List<Task>();
        // Adding a stopwatch. 
        public static void measureTime(List<string> times, Stopwatch stopWatch)
        {
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            times.Add(String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10));
        }

        // Program checking if the datatype <long> is a prime number. 
        public static bool IsPrime(long number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (long)Math.Floor(Math.Sqrt(number));

            for (long i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return false;

            return true;
        }

        // Sequential Implementation. 
        public static List<long> GetPrimesSequential(long first, long last)
        {
            List<long> result = new List<long>();

            for (long i = first; i <= last; i++)
            {
                if (IsPrime(i))
                {
                    result.Add(i);
                }
            }

            return result;
        }

        // Parallel Implementation. 
        public static List<long> GetPrimesParallel(long first, long last)
        {
            List<long> result = new List<long>();
            int maxTasks = Environment.ProcessorCount;
            long range = last - first;
            long segmentSize = range / maxTasks + 1;

            for (int i = 0; i < maxTasks; i++) {
                Task task = Task.Run(() => {
                    long start = first + (i * segmentSize) - (i == 0 ? 0 : 1);
                    long end = first + ((i + 1) * segmentSize) - 2;
                    var partition = GetPrimesSequential(start, end);
                    lock (objLock)
                    {
                        result.AddRange(partition);
                    }
                });
                tasks.Add(task);
            }

            return result;
        }
    }
}
