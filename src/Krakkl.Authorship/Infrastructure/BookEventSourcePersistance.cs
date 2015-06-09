using Krakkl.Authorship.Core;

namespace Krakkl.Authorship.Infrastructure
{
    internal class BookEventSourcePersistance
    {
        public void OnBookCreated(object sender, BookCreatedEventArgs e)
        {
            var x = e.BookKey;
        }
    }
}
