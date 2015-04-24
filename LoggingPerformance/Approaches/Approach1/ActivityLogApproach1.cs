using System.IO;
using System.IO.Compression;
using LoggingPerformance.Octopus;
using ProtoBuf;

namespace LoggingPerformance.Approaches.Approach1
{
    public class ActivityLogApproach1
    {
        const string V1ProtocolBuffersWithGzipCompression = "v1";

        public ActivityLogApproach1(string id)
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
            using (var zip = new GZipStream(memory, CompressionMode.Decompress))
            {
                var result = Serializer.Deserialize<ActivityLogTreeNode>(zip);
                return result;
            }
        }

        public void Serialize(ActivityLogTreeNode rootNode)
        {
            var memory = new MemoryStream();
            using (var zip = new GZipStream(memory, CompressionMode.Compress))
            {
                Serializer.Serialize(zip, rootNode);
                zip.Flush();
            }

            LogData = memory.ToArray();
            Format = V1ProtocolBuffersWithGzipCompression;
        }
    }
}
