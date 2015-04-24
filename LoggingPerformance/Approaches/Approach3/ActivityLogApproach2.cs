using System.IO;
using System.IO.Compression;
using LoggingPerformance.Octopus;
using ProtoBuf;

namespace LoggingPerformance.Approaches.Approach3
{
    public class ActivityLogApproach3
    {
        const string V1ProtocolBuffersWithGzipCompression = "v1";

        public ActivityLogApproach3(string id)
        {
            Format = V1ProtocolBuffersWithGzipCompression;
            Id = id;
        }

        public string Id { get; protected set; }

        public string LogData { get; set; }
        public string Format { get; set; }

        public ActivityLogTreeNode Deserialize()
        {
            if (Format != V1ProtocolBuffersWithGzipCompression)
            {
                throw new InvalidDataException("Unknown activity log format: " + Format);
            }

            if (LogData == null || LogData.Length == 0)
            {
                return new ActivityLogTreeNode() { CorrelationId = Id };
            }

            return new ActivityLogTreeNode();
        }

        public void Serialize(ActivityLogTreeNode rootNode)
        {
            Format = V1ProtocolBuffersWithGzipCompression;


        }
    }
}
