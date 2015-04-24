using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using LoggingPerformance.Octopus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EncodingPerformance.Encoders
{
    public class PaulJArrayFastEncoder : ILogEncoder
    {
        public void Encode(ActivityLogEntry entry, StringBuilder builder)
        {
            builder.Append("[");
            builder.Append("\"").Append(entry.CorrelationId).Append("\",");
            builder.Append("\"").Append(entry.Category).Append("\",");
            builder.Append("\"").Append(entry.Occurred.ToString("O")).Append("\",");
            builder.Append("\"").Append(HttpUtility.JavaScriptStringEncode(entry.Message)).Append("\",");
            builder.Append("\"").Append(HttpUtility.JavaScriptStringEncode(entry.Detail ?? "")).Append("\",");
            builder.Append(entry.Percentage != null ? entry.Percentage.ToString() : "null");
            builder.Append("]\n");
        }

        public IEnumerable<ActivityLogEntry> DecodeAll(string input)
        {
            using (var reader = new JsonTextReader(new StringReader(input)))
            {
                reader.SupportMultipleContent = true;

                while (reader.Read())
                {
                    if (reader.TokenType != JsonToken.StartArray)
                        continue;

                    var array = (JArray)JToken.ReadFrom(reader);
                    var dt = array[2].ToObject<DateTimeOffset>();
                    var id = array[0].Value<string>();
                    var category = (ActivityLogEntryCategory)(Enum.Parse(typeof(ActivityLogEntryCategory), array[1].Value<string>()));
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