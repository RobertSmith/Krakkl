using System;

namespace Krakkl.Infrastructure.Authorship.Book.EventTranslator
{
    public class FailedTranslationMessage
    {
        public string EventData { get; set; }
        public string Exception { get; set; }
        public DateTime TimeStamp => DateTime.UtcNow;
    }
}