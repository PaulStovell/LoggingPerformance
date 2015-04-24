using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace LoggingPerformance.Octopus
{
    [ProtoContract]
    public class ActivityLogTreeNode
    {
        public ActivityLogTreeNode()
        {
            LogEntries = new List<ActivityLogEntry>();
            Children = new List<ActivityLogTreeNode>();
        }

        public ActivityLogTreeNode(string correlationId, IEnumerable<ActivityLogEntry> logEntries, IEnumerable<ActivityLogTreeNode> children)
        {
            CorrelationId = correlationId;
            LogEntries = new List<ActivityLogEntry>(logEntries);
            Children = new List<ActivityLogTreeNode>(children);
        }

        [ProtoMember(1)]
        public string CorrelationId { get; set; }

        [ProtoMember(2)]
        public List<ActivityLogTreeNode> Children { get; set; }

        [ProtoMember(3)]
        public List<ActivityLogEntry> LogEntries { get; set; }

        public ActivityLogTreeNode FindOrAdd(string candidateCorrelationId)
        {
            if (candidateCorrelationId == CorrelationId)
            {
                return this;
            }

            foreach (var child in Children)
            {
                if (candidateCorrelationId.StartsWith(child.CorrelationId))
                {
                    var found = child.FindOrAdd(candidateCorrelationId);
                    if (found != null)
                        return found;
                }
            }

            var childPath = candidateCorrelationId.Substring(CorrelationId.Length + 1);
            var childName = childPath.Contains('/') ? childPath.Substring(0, childPath.IndexOf('/')) : childPath;
            var newChild = new ActivityLogTreeNode { CorrelationId = CorrelationId + "/" + childName };
            Children.Add(newChild);
            return newChild.FindOrAdd(candidateCorrelationId);
        }

        public ActivityLogTreeNode Clone()
        {
            return new ActivityLogTreeNode(CorrelationId, LogEntries, Children.Select(c => c.Clone()));
        }
    }
}