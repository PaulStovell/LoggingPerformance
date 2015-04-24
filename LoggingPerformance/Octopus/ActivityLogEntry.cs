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

        public ActivityLogEntry(string correlationId, DateTimeOffset occurred, ActivityLogEntryCategory category, string message, string detail = null, int? percentage = null)
        {
            CorrelationId = correlationId;
            Occurred = occurred;
            Category = category;
            Message = message;
            Detail = detail;
            Percentage = percentage;
        }

        public string CorrelationId { get; set; }

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