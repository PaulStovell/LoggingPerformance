using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LoggingPerformance.TestHarness
{
    public static class Timings
    {
        static readonly ConcurrentDictionary<string, Stopwatch> watches = new ConcurrentDictionary<string, Stopwatch>();
        static readonly ConcurrentDictionary<string, int> counters = new ConcurrentDictionary<string, int>();

        public static void Reset()
        {
            watches.Clear();
        }

        public static void Time(string name, Action callback)
        {
            var watch = watches.GetOrAdd(name, _ => new Stopwatch());
            watch.Start();
            callback();
            watch.Stop();
        }

        public static Dictionary<string, TimeSpan> GetTimes()
        {
            return watches.ToDictionary(t => t.Key, t => t.Value.Elapsed);
        }

        public static void Increment(string counter)
        {
            counters.AddOrUpdate(counter, n => 1, (n, i) => i + 1);
        }

        public static Dictionary<string, int> GetCounters()
        {
            return counters.ToDictionary(t => t.Key, t => t.Value);
        }
    }
}