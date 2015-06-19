using System.Collections.Generic;
using System.Threading.Tasks;
using Krakkl.Query.Models;
using Newtonsoft.Json;

namespace Krakkl.Query
{
    public class Books : OrchestrateBase
    {
        public async Task<IEnumerable<BookModel>> GetAuthorBooksAsyc(string authorId)
        {
            var query =  await Orchestrate.SearchAsync("Books", "Authors.Key:" + authorId, 100);
            var bookList = new List<BookModel>();

            foreach (var result in query.Results)
            {
                var book = JsonConvert.DeserializeObject<BookModel>(result.Value.ToString());
                book.Key = result.Path.Key;
                bookList.Add(book);
            }

            return bookList;
        }

        public async Task<BookModel> GetBookAsync(string key)
        {
            var query = await Orchestrate.GetAsync("Books", key);
            return JsonConvert.DeserializeObject<BookModel>(query.Value.ToString());
        }
    }
}
