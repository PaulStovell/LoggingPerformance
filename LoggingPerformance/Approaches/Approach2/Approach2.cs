using LoggingPerformance.Octopus;
using LoggingPerformance.Octopus.Persistance;
using LoggingPerformance.TestHarness;

namespace LoggingPerformance.Approaches.Approach2
{
    public class Approach2 : Approach
    {
        public Approach2()
            : base("Aproach 2: Same as 1, but without zip")
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
                return txn.ExecuteScalar<long>("SELECT LEN(LogData) from ActivityLog_Approach2 where Id = @id",
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