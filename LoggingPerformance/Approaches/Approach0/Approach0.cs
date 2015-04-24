using LoggingPerformance.Approaches.Approach1;
using LoggingPerformance.Octopus;
using LoggingPerformance.Octopus.Persistance;
using LoggingPerformance.TestHarness;

namespace LoggingPerformance.Approaches.Approach0
{
    public class Approach0 : Approach
    {
        public Approach0()
            : base("Aproach 0: All in memory")
        {
        }

        public override IServerLogStorage GetStorage()
        {
            return new InMemoryLogStorage();
        }

        public override long GetDiskImpact(string id)
        {
            return 0;
        }

        public override void CountAnyAdditionalWork(string id)
        {
            
        }

        public override void TransitionToDurableStorage(string id)
        {
            // This strategy protobufs and GZIPs immediately, so there is nothing to transition
        }
    }
}