using System;

namespace Krakkl.Query.Models
{
    public class ModerationIssueModel
    {
        public string Field { get; set; }
        public string Reason { get; set; }
        public string IssueNotedBy { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ClearedAt { get; set; }
    }
}