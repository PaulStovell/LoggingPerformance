using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using LoggingPerformance.Octopus;

namespace EncodingPerformance.Encoders
{
    public class XmlLogEncoder : ILogEncoder
    {
        public void Encode(ActivityLogEntry entry, StringBuilder builder)
        {
            var element = new XElement("L",
                new XAttribute("i", entry.CorrelationId),
                new XAttribute("c", entry.Category),
                new XAttribute("o", entry.Occurred.ToString("O")),
                new XAttribute("m", entry.Message)
                );

            if (entry.Percentage != null) element.Add(new XAttribute("p", entry.Percentage));
            if (entry.Detail != null) element.Add(new XAttribute("d", entry.Detail));

            builder.Append(element).Append("\n");
        }

        public IEnumerable<ActivityLogEntry> DecodeAll(string input)
        {
            using (var reader = new XmlTextReader(new StringReader("<Logs>" + input + "</Logs>")))
            {
                reader.Read();

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        var e = (XElement)XNode.ReadFrom(reader);
                        var id = e.Attribute("i").Value;
                        var category = (ActivityLogEntryCategory)(Enum.Parse(typeof(ActivityLogEntryCategory), e.Attribute("c").Value));
                        var message = e.Attribute("m").Value;
                        var occurred = DateTimeOffset.Parse(e.Attribute("o").Value);

                        var detailAttribute = e.Attribute("d");
                        var progressAttribute = e.Attribute("p");

                        var detail = detailAttribute != null && !string.IsNullOrWhiteSpace(detailAttribute.Value)
                            ? detailAttribute.Value
                            : null;

                        var progress = progressAttribute != null && !string.IsNullOrWhiteSpace(progressAttribute.Value)
                            ? int.Parse(progressAttribute.Value)
                            : (int?)null;

                        yield return new ActivityLogEntry(id, occurred, category, message, detail, progress);
                    }
                }
            }
        }
    }
}