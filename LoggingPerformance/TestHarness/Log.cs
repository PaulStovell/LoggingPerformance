using System;
using System.Threading;
using LoggingPerformance.Octopus;

namespace LoggingPerformance.TestHarness
{
    public class Log : IDisposable
    {
        private readonly IServerLogStorage storage;
        private readonly string id;
        
        public Log(string id, IServerLogStorage storage)
        {
            this.id = id;
            this.storage = storage;
        }

        public Log Indent()
        {
            return new Log(id + "/" + Guid.NewGuid(), storage);
        }

        public void Info(string message)
        {
            Timings.Time("Log an info message", delegate
            {
                storage.Append(id, new ActivityLogEntry(id, DateTimeOffset.UtcNow, ActivityLogEntryCategory.Info, message));
            });
        }
        
        public void Error(string message)
        {
            Timings.Time("Log an error", delegate
            {
                storage.Append(id, new ActivityLogEntry(id, DateTimeOffset.UtcNow, ActivityLogEntryCategory.Error, message));
            });
        }

        public void Progress(int progress, string message)
        {
            Timings.Time("Log a progress entry", delegate
            {
                storage.Append(id, new ActivityLogEntry(id, DateTimeOffset.UtcNow, ActivityLogEntryCategory.Updated, message, percentage: progress));
            });
        }

        public void Dispose()
        {
        }
    }
}