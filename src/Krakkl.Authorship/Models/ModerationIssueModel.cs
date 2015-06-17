namespace Krakkl.Authorship.Models
{
    public sealed class ModerationIssueModel
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
