using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Krakkl.Query.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
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
            var book = JsonConvert.DeserializeObject<BookModel>(query.Value.ToString());
            book.Key = query.Path.Key;

            return book;
        }

        public List<string> GetImagesByBook(string bookKey)
        {
            var uris = new List<string>();

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_storageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(bookKey);

            uris.AddRange(blobContainer.ListBlobs().Select(x => x.Uri.ToString()));

            return uris;
        }
    }
}
