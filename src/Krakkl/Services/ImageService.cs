using System.Collections.Generic;
using System.IO;
using Krakkl.Query;

namespace Krakkl.Services
{
    public class ImageService
    {
//        private readonly BookImageService _service = new BookImageService(new Authorship.Infrastructure.BookAggregateRepository());

        public List<string> GetBookImageUrls(string bookKey)
        {
            var query = new BookQueries();
            return query.GetImagesByBook(bookKey);
        }

        public void AddImage(string bookKey, Stream image)
        {
//            _service.
        }
    }
}