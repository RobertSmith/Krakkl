using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Krakkl.Authorship.Book.Aggregate;
using Krakkl.Authorship.Book.Repository;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace Krakkl.Authorship.Infrastructure
{
    public class BookAggregateRepository : IBookAggregateRepository
    {
        private readonly MessagingService _messagingService;
        private readonly Orchestrate.Net.Orchestrate _orchestrate;
        private readonly string _storageConnectionString;
        private const string BookEventsCollection = "BookEvents";

        public BookAggregateRepository()
        {
            var configuration = new Configuration().AddJsonFile("config.json");

            _orchestrate = new Orchestrate.Net.Orchestrate(configuration["Data:Orchestrate:ApiKey"]);
            _storageConnectionString = configuration["Data:AzureStorage:ConnectionString"];
            _messagingService = new MessagingService();
        }

        public BookAggregate FindByKey<T>(Guid key)
        {
            var aggregate = (BookAggregate)BookAggregateCache.Get(key);

            if (aggregate != null)
                return aggregate;

            Entities.Book book = GetBookSnapshot(key);

            const int limit = 20;
            var offset = 0;
            var more = true;
            var events = new List<object>();
            var minRefTime = book.LastEventRefTime;
            var searchQuery = $"BookKey:{key} AND @path.reftime:[{minRefTime + 1} TO {long.MaxValue}]";

            while (more)
            {
                var query = _orchestrate.Search(BookEventsCollection, searchQuery, limit, offset, "@path.reftime:asc");
                var searchResults = query.Results.ToList();

                if (!searchResults.Any())
                    break;

                foreach (var result in searchResults)
                {
                    book.LastEventRefTime = long.Parse(result.Path.RefTime);

                    var i = JsonConvert.DeserializeObject<BookEventArgs>(result.Value.ToString());

                    switch (i.EventType)
                    {
                        case "BookCreated":
                            events.Add(JsonConvert.DeserializeObject<BookCreatedEventArgs>(result.Value.ToString()));
                            break;

                        case "AuthorAddedToBook":
                            events.Add(JsonConvert.DeserializeObject<AuthorAddedToBookEventArgs>(result.Value.ToString()));
                            break;

                        case "AuthorRemovedFromBook":
                            events.Add(JsonConvert.DeserializeObject<AuthorRemovedFromBookEventArgs>(result.Value.ToString()));
                            break;

                        case "BookRetitled":
                            events.Add(JsonConvert.DeserializeObject<BookRetitledEventArgs>(result.Value.ToString()));
                            break;

                        case "BookSubTitleChanged":
                            events.Add(JsonConvert.DeserializeObject<BookSubTitleChangedEventArgs>(result.Value.ToString()));
                            break;

                        case "BookSeriesTitleChanged":
                            events.Add(JsonConvert.DeserializeObject<BookSeriesTitleChangedEventArgs>(result.Value.ToString()));
                            break;

                        case "BookSeriesVolumeChanged":
                            events.Add(JsonConvert.DeserializeObject<BookSeriesVolumeChangedEventArgs>(result.Value.ToString()));
                            break;

                        case "BookGenreChanged":
                            events.Add(JsonConvert.DeserializeObject<BookGenreChangedEventArgs>(result.Value.ToString()));
                            break;

                        case "BookLanguageChanged":
                            events.Add(JsonConvert.DeserializeObject<BookLanguageChangedEventArgs>(result.Value.ToString()));
                            break;

                        case "BookSynopsisUpdated":
                            events.Add(JsonConvert.DeserializeObject<BookSynopsisUpdatedEventArgs>(result.Value.ToString()));
                            break;

                        case "BookCompleted":
                            events.Add(JsonConvert.DeserializeObject<BookCompletedEventArgs>(result.Value.ToString()));
                            break;

                        case "BookAbandoned":
                            events.Add(JsonConvert.DeserializeObject<BookAbandonedEventArgs>(result.Value.ToString()));
                            break;

                        case "BookRevived":
                            events.Add(JsonConvert.DeserializeObject<BookRevivedEventArgs>(result.Value.ToString()));
                            break;

                        case "BookPublished":
                            events.Add(JsonConvert.DeserializeObject<BookPublishedEventArgs>(result.Value.ToString()));
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(query.Next))
                    offset += limit;
                else
                    more = false;
            }

            var newAggregate = new BookAggregate(book, events);

            if (events.Count >= 10)
                TakeBookSnapshot(newAggregate);

            return newAggregate;
        }

        public void Save(BookAggregate aggregate)
        {
            for (var x = aggregate.UncommittedEvents.Count; x > 0; x--)
            {
                _orchestrate.Post(BookEventsCollection, aggregate.UncommittedEvents[x - 1]);
                _messagingService.SendBookEventMessage(aggregate.UncommittedEvents[x - 1]);
                aggregate.UncommittedEvents.RemoveAt(x - 1);
            }

            BookAggregateCache.UpdateItem(aggregate.Key, aggregate);
        }

        public async void SaveCoverArt(BookAggregate aggregate, Guid coverArtKey, Stream coverArt)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_storageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(aggregate.Key.ToString());

            await container.CreateIfNotExistsAsync();
            var permissions = new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob };
            await container.SetPermissionsAsync(permissions);

            CloudBlockBlob blob = container.GetBlockBlobReference(coverArtKey.ToString());
            await blob.UploadFromStreamAsync(coverArt);
        }

        private Entities.Book GetBookSnapshot(Guid key)
        {
            var searchResult = _orchestrate.Search(Definitions.BookSnapshotCollection, "@path.key:" + key);

            if (searchResult.Count == 0)
                return new Entities.Book();

            return JsonConvert.DeserializeObject<Entities.Book>(searchResult.Results.First().Value.ToString());
        }

        private void TakeBookSnapshot(BookAggregate aggregate)
        {
            _orchestrate.Put(Definitions.BookSnapshotCollection, aggregate.Key.ToString(), aggregate.Book);
        }
    }
}