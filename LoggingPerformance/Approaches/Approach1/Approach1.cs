using LoggingPerformance.Octopus;
using LoggingPerformance.Octopus.Persistance;
using LoggingPerformance.TestHarness;

namespace LoggingPerformance.Approaches.Approach1
{
    public class Approach1 : Approach
    {
        public Approach1() : base("Aproach 1: Octopus 3.0 original stype (protobuf+gzip, buffer and flush every second")
        {
        }

        public override IServerLogStorage GetStorage()
        {
            return new RelationalServerLogStorage(IntegrationTestDatabase.Store);
        }

        public override long GetDiskImpact(string id)
        {
            using (var txn = IntegrationTestDatabase.Store.BeginTransaction())
            {
                return txn.ExecuteScalar<long>("SELECT LEN(LogData) from ActivityLog_Approach1 where Id = @id",
                    new CommandParameters(new {id = id}));
            }
        }

        public override void CountAnyAdditionalWork(string id)
        {
            // With this strategy, we flush our buffer every second. Assuming a deployment lasts 5 minutes, 
            // we would do this 60 * 60 * 5 times. It's only fair that we count this flushing cost since it 
            // uses CPU and IO.
            var storage = new RelationalServerLogStorage(IntegrationTestDatabase.Store);
            Timings.Time("Flushing with ProtocolBuffers", delegate
            {
                for (var i = 0; i < 60 * 60 * 5; i++)
                {
                    storage.ForceFlush(id);
                }
            });
        }

        public override void TransitionToDurableStorage(string id)
        {
            // This strategy protobufs and GZIPs immediately, so there is nothing to transition
        }
    }
}