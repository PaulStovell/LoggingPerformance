using LoggingPerformance.Octopus.Persistance;

namespace LoggingPerformance.Approaches.Approach2
{
    public class ActivityLogApproach2Map : DocumentMap<ActivityLogApproach2>
    {
        public ActivityLogApproach2Map()
        {
            TableName = "ActivityLog_Approach2";
            Column(m => m.LogData).Nullable();
            Column(m => m.Format).Nullable().WithMaxLength(10);
        }
    }
}
