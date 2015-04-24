using System;
using System.Collections.Generic;
using System.Threading;
using LoggingPerformance.Approaches.Approach0;
using LoggingPerformance.Approaches.Approach1;
using LoggingPerformance.Approaches.Approach2;
using LoggingPerformance.Approaches.Approach3;
using LoggingPerformance.TestHarness;
using NUnit.Framework;

namespace LoggingPerformance
{
    [TestFixture]
    public class Tests
    {
        private static readonly IEnumerable<Approach> approaches = new Approach[]
        {
            new Approach0(),
            new Approach1(),
            new Approach2(),
            new Approach3()
        };
            
        [Test]
        [TestCaseSource("approaches")]
        public void Test(Approach approach)
        {
            var storage = approach.GetStorage();

            Console.WriteLine("Run deployment");
            var testDeployment = new TestDeployment(new Log("tasks-1", storage));
            testDeployment.Deploy();

            Thread.Sleep(3000);

            Console.WriteLine("Read log first time");
            Timings.Time("First read of the log", delegate
            {
                storage.GetLog("tasks-1");
            });

            Console.WriteLine("Read log subsequent times");
            for (var i = 0; i < 20; i++)
            {
                Timings.Time("Subsequent reads of the log", delegate
                {
                    Assert.IsNotNull(storage.GetLog("tasks-1"));
                });
            }

            Console.WriteLine("Count additional work");
            approach.CountAnyAdditionalWork("tasks-1");

            Console.WriteLine("Get disk impact");
            var transientImpact = approach.GetDiskImpact("tasks-1");

            Console.WriteLine("Transition to durable");
            approach.TransitionToDurableStorage("tasks-1");

            Console.WriteLine("Get disk impact");
            var longTermImpact = approach.GetDiskImpact("tasks-1");

            Console.WriteLine(approach.Name);
            foreach (var timing in Timings.GetTimes())
            {
                Console.WriteLine(" - " + timing.Key.PadRight(20, ' ') + ": " + timing.Value.ToString().PadRight(12, ' '));
            }
            foreach (var timing in Timings.GetCounters())
            {
                Console.WriteLine(" - " + timing.Key.PadRight(20, ' ') + ": " + timing.Value.ToString().PadRight(12, ' '));
            }
            Console.WriteLine(" - Short term disk impact:  " + transientImpact);
            Console.WriteLine(" - Long term disk impact :  " + longTermImpact);


            if (storage is IDisposable)
            {
                ((IDisposable)storage).Dispose();
            }
        }
    }
}
