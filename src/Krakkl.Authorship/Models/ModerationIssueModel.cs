namespace Krakkl.Authorship.Models
{
    internal sealed class ModerationIssueModel
    {
        public string Field { get; set; }
        public string Reason { get; set; }

        public ModerationIssueModel(string field, string reason)
        {
            Field = field;
            Reason = reason;
        }
    }
}
