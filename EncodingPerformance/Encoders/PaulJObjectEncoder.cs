using System.Collections.Generic;
using System.IO;
using System.Text;
using LoggingPerformance.Octopus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EncodingPerformance.Encoders
{
    public class PaulJObjectEncoder : ILogEncoder
    {
        public void Encode(ActivityLogEntry entry, StringBuilder builder)
        {
            builder.Append(JObject.FromObject(entry));
            builder.Append("\n");
        }

        public IEnumerable<ActivityLogEntry> DecodeAll(string input)
        {
            using (var reader = new JsonTextReader(new StringReader(input)))
            {
                reader.SupportMultipleContent = true;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        var x = JToken.ReadFrom(reader);
                        var entry = x.ToObject<ActivityLogEntry>();
                        yield return entry;
                    }
                }
            }

            yield break;
        }
    }
}