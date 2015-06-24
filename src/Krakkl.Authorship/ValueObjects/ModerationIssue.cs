using System;

namespace Krakkl.Authorship.ValueObjects
{
    public sealed class ModerationIssue
    {
        public string Field { get; set; }
        public string Reason { get; set; }
        public string IssueNotedBy { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ClearedAt { get; set; }

        public ModerationIssue(string field, string reason)
        {
            Field = field;
            Reason = reason;
            Status = 2; // Noted
            CreatedAt = DateTime.UtcNow;
        }
    }
}
