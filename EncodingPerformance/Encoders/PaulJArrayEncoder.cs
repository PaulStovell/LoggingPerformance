using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LoggingPerformance.Octopus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EncodingPerformance.Encoders
{
    public class PaulJArrayEncoder : ILogEncoder
    {
        public void Encode(ActivityLogEntry entry, StringBuilder builder)
        {
            builder.Append(new JArray(entry.CorrelationId, entry.Category.ToString(), entry.Occurred, entry.Message, entry.Detail, entry.Percentage));
            builder.Append("\n");
        }

        public IEnumerable<ActivityLogEntry> DecodeAll(string input)
        {
            using (var reader = new JsonTextReader(new StringReader(input)))
            {
                reader.SupportMultipleContent = true;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        var array = (JArray)JToken.ReadFrom(reader);

                        var dt = array[2].ToObject<DateTimeOffset>();
                        var id = array[0].Value<string>();
                        var category = (ActivityLogEntryCategory)(Enum.Parse(typeof (ActivityLogEntryCategory), array[1].Value<string>()));
                        var occurred = dt;
                        var message = array[3].Value<string>();
                        var detail = array[4].Value<string>();
                        var progress = array[5].Value<int?>();

                        var entry = new ActivityLogEntry(id, occurred, category, message, detail, progress);
                        yield return entry;
                    }
                }
            }
        }
    }
}