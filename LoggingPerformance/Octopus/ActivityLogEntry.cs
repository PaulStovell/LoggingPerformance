using System;
using ProtoBuf;

namespace LoggingPerformance.Octopus
{
    [ProtoContract]
    public class ActivityLogEntry
    {
        public ActivityLogEntry()
        {
        }

        public ActivityLogEntry(DateTimeOffset occurred, ActivityLogEntryCategory category, string message, string detail = null, int? percentage = null)
        {
            Occurred = occurred;
            Category = category;
            Message = message;
            Detail = detail;
            Percentage = percentage;
        }

        [ProtoMember(1)]
        public DateTimeOffset Occurred { get; set; }

        [ProtoMember(2)]
        public ActivityLogEntryCategory Category { get; set; }

        [ProtoMember(3)]
        public string Message { get; set; }

        [ProtoMember(4)]
        public int? Percentage { get; set; }

        [ProtoMember(5)]
        public string Detail { get; set; }
    }
}