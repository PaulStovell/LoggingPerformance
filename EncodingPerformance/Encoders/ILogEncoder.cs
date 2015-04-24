using System.Collections.Generic;
using System.Text;
using LoggingPerformance.Octopus;

namespace EncodingPerformance.Encoders
{
    public interface ILogEncoder
    {
        void Encode(ActivityLogEntry entry, StringBuilder builder);
        IEnumerable<ActivityLogEntry> DecodeAll(string input);
    }
}