using System.Collections.Generic;
using LoggingPerformance.Octopus;

namespace LoggingPerformance.Approaches.Approach0
{
    public class InMemoryLogStorage : IServerLogStorage
    {
        public InMemoryLogStorage()
        {
        }

        public void Append(string correlationId, ActivityLogEntry entry)
        {
            
        }

        public IList<ActivityLogTreeNode> GetLog(string correlationId)
        {
            return new ActivityLogTreeNode[0];
        }
    }
}
