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
            builder.Append("\"").Append(CategoryEnumToString(entry.Category)).Append("\",");
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
                    var id = array[0].Value<string>();
                    var category = CategoryStringToEnum(array[1].Value<string>());
                    var occurred = array[2].ToObject<DateTimeOffset>();
                    var message = array[3].Value<string>();
                    var detail = array[4].Value<string>();
                    var progress = array[5].Value<int?>();

                    var entry = new ActivityLogEntry(id, occurred, category, message, detail, progress);
                    yield return entry;
                }
            }
        }

        static string CategoryEnumToString(ActivityLogEntryCategory value)
        {
            switch (value)
            {
                case ActivityLogEntryCategory.Trace: return "TRA";
                case ActivityLogEntryCategory.Verbose: return "VBS";
                case ActivityLogEntryCategory.Info: return "INF";
                case ActivityLogEntryCategory.Alert: return "ALR";
                case ActivityLogEntryCategory.Warning: return "WRN";
                case ActivityLogEntryCategory.Error: return "ERR";
                case ActivityLogEntryCategory.Fatal: return "FAT";
                case ActivityLogEntryCategory.Planned: return "PLN";
                case ActivityLogEntryCategory.Updated: return "UPD";
                case ActivityLogEntryCategory.Finished: return "FIN";
                case ActivityLogEntryCategory.Abandoned: return "ABN";
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }

        static ActivityLogEntryCategory CategoryStringToEnum(string value)
        {
            switch (value)
            {
                case "TRA": return ActivityLogEntryCategory.Trace;
                case "VBS": return ActivityLogEntryCategory.Verbose;
                case "INF": return ActivityLogEntryCategory.Info;
                case "ALR": return ActivityLogEntryCategory.Alert;
                case "WRN": return ActivityLogEntryCategory.Warning;
                case "ERR": return ActivityLogEntryCategory.Error;
                case "FAT": return ActivityLogEntryCategory.Fatal;
                case "PLN": return ActivityLogEntryCategory.Planned;
                case "UPD": return ActivityLogEntryCategory.Updated;
                case "FIN": return ActivityLogEntryCategory.Finished;
                case "ABN": return ActivityLogEntryCategory.Abandoned;
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }
    }
}