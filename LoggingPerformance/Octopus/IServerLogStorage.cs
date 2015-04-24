using System.Collections.Generic;

namespace LoggingPerformance.Octopus
{
    public interface IServerLogStorage
    {
        void Append(string correlationId, ActivityLogEntry entry);
        IList<ActivityLogTreeNode> GetLog(string correlationId);
    }

    // This is a union of TraceCategory and
    // ProgressMessageCategory - it isn't clear yet
    // yet whether these two enumerations should
    // be separate.
}
