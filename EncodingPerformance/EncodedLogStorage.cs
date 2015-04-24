using System;
using System.Collections.Generic;
using System.Text;
using EncodingPerformance.Encoders;
using LoggingPerformance.Octopus;

namespace EncodingPerformance
{
    public class EncodedLogStorage : IServerLogStorage
    {
        private readonly ILogEncoder encoder;
        readonly StringBuilder output = new StringBuilder();

        public EncodedLogStorage(ILogEncoder encoder)
        {
            this.encoder = encoder;
        }

        public void Append(string correlationId, ActivityLogEntry entry)
        {
            encoder.Encode(entry, output);
        }

        public IList<ActivityLogTreeNode> GetLog(string correlationId)
        {
            throw new NotImplementedException();
        }

        public string GetCompleteLog()
        {
            return output.ToString();
        }
    }
}