using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using LoggingPerformance.Octopus;

namespace LoggingPerformance.TestHarness
{
    public static class Renderer
    {
        public static string RenderLog(IList<ActivityLogTreeNode> log)
        {
            var result = new StringBuilder();

            foreach (var node in log)
            {
                RenderNode(result, node, "| ");
            }

            return result.ToString();
        }

        static void RenderNode(StringBuilder result, ActivityLogTreeNode activityElement, string indent)
        {
            result
                .Append("  ")
                .Append(indent)
                .Append(": ")
                .Append(activityElement.CorrelationId);

            result.AppendLine();

            foreach (var entry in activityElement.LogEntries)
            {
                var entryWithDetail = string.Join(Environment.NewLine, new[] { entry.Message, entry.Detail }.Where(d => d != null));
                var lines = entryWithDetail.Split(Environment.NewLine.ToCharArray()).Select(l => l.Trim()).Where(l => l.Length > 0).Select(l => indent + "  " + l).ToList();
                var firstLine = lines.FirstOrDefault();
                if (firstLine != null)
                {
                    result.Append(entry.Category).AppendLine(firstLine);
                }
            }

            if (activityElement.Children == null)
                return;

            foreach (var child in activityElement.Children)
            {
                RenderNode(result, child, indent + "  ");
            }
        }
    }
}
