using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Krakkl.Query
{
    public class Books : OrchestrateBase
    {
        public IEnumerable<BookModel> GetAuthorBooks(string authorId)
        {
            var query = Orchestrate.Search("Books", "AuthorId:" + authorId, 100);
            var bookList = new List<BookModel>();

            foreach (var result in query.Results)
            {
                var book = JsonConvert.DeserializeObject<BookModel>(result.Value.ToString());
                book.Key = result.Path.Key;
                bookList.Add(book);
            }

            return bookList;
        }

        public BookModel GetBook(string key)
        {
            var query = Orchestrate.Get("Books", key);
            return JsonConvert.DeserializeObject<BookModel>(query.Value.ToString());
        }

        public async Task<BookModel> GetBookAsync(string key)
        {
            var query = await Orchestrate.GetAsync("Books", key);
            return JsonConvert.DeserializeObject<BookModel>(query.Value.ToString());
        }
    }

    public class BookModel
    {
        public string Key { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string SeriesTitle { get; set; }
        public string SeriesVolume { get; set; }
        public string GenreKey { get; set; }
        public string Genre { get; set; }
        public string LanguageKey { get; set; }
        public string Language { get; set; }
        public string CoverArt { get; set; }
        public string Synopsis { get; set; }
        public bool Public { get; set; }
        public bool Complete { get; set; }
        public bool Abandoned { get; set; }
        public bool DMCA { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }
}
