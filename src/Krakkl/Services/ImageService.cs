using System.Collections.Generic;
using Krakkl.Authorship.Services;
using Krakkl.Query;

namespace Krakkl.Services
{
    public class ImageService
    {
        private readonly BookService _service = new BookService(new Authorship.Infrastructure.BookAggregateRepository());

        public List<string> GetBookImageUrls(string bookKey)
        {
            var query = new Books();
            return query.GetImagesByBook(bookKey);
        }
    }
}