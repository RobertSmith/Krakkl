using System;

namespace Krakkl.Authorship.Book.Models
{
    public sealed class ModerationIssueModel
    {
        public string Field { get; set; }
        public string Reason { get; set; }
        public string IssueNotedBy { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ClearedAt { get; set; }

        public ModerationIssueModel(string field, string reason)
        {
            Field = field;
            Reason = reason;
            Status = 2; // Noted
            CreatedAt = DateTime.UtcNow;
        }
    }
}
