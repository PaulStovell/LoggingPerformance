using System.IO;
using System.IO.Compression;
using LoggingPerformance.Octopus;
using ProtoBuf;

namespace LoggingPerformance.Approaches.Approach2
{
    public class ActivityLogApproach2
    {
        const string V1ProtocolBuffersWithGzipCompression = "v1";

        public ActivityLogApproach2(string id)
        {
            Format = V1ProtocolBuffersWithGzipCompression;
            Id = id;
        }

        public string Id { get; protected set; }

        public byte[] LogData { get; set; }
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

            var memory = new MemoryStream(LogData);
            {
                var result = Serializer.Deserialize<ActivityLogTreeNode>(memory);
                return result;
            }
        }

        public void Serialize(ActivityLogTreeNode rootNode)
        {
            var memory = new MemoryStream();
            {
                Serializer.Serialize(memory, rootNode);
            }

            LogData = memory.ToArray();
            Format = V1ProtocolBuffersWithGzipCompression;
        }
    }
}
