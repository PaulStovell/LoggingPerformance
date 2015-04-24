using LoggingPerformance.Octopus.Persistance;

namespace LoggingPerformance.Approaches.Approach1
{
    public class ActivityLogApproach1Map : DocumentMap<ActivityLogApproach1>
    {
        public ActivityLogApproach1Map()
        {
            TableName = "ActivityLog_Approach1";
            Column(m => m.LogData).Nullable();
            Column(m => m.Format).Nullable().WithMaxLength(10);
        }
    }
}
