using LoggingPerformance.Octopus.Persistance;

namespace LoggingPerformance.Approaches.Approach3
{
    public class ActivityLogApproach3Map : DocumentMap<ActivityLogApproach3>
    {
        public ActivityLogApproach3Map()
        {
            TableName = "ActivityLog_Approach3";
            Column(m => m.LogData).Nullable();
            Column(m => m.Format).Nullable().WithMaxLength(10);
        }
    }
}
