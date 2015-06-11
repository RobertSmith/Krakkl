using Krakkl.Authorship.Core;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Krakkl.Authorship.Infrastructure
{
    internal class BookEventSourcePersistence
    {
        private readonly CloudQueue _queue;

        public BookEventSourcePersistence()
        {
            var configuration = new Configuration().AddJsonFile("config.json");
            var storageConnectionString = configuration["Data:AzureStorage:ConnectionString"];

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference("authorship");
            _queue.CreateIfNotExistsAsync();
        }

        public async void OnBookPersistenceEvent(object sender, object e)
        {
            CloudQueueMessage message = new CloudQueueMessage(e.ToString());
            await _queue.AddMessageAsync(message);
        }

        public void OnAuthorRemovedFromBook(object sender, AuthorRemovedFromBookEventArgs e)
        {
            CloudQueueMessage message = new CloudQueueMessage(e.ToString());
            _queue.AddMessageAsync(message);

//            var patchItems = new List<object>
//            {
//                new PatchItemObject { Op = "replace", Path = "/Authors", Value = e.ValidAuthors },
//                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
//                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
//            };
//
//            PatchBook(patchItems, e.BookKey.ToString());
//            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
        }

//        public void OnBookRetitled(object sender, BookRetitledEventArgs e)
//        {
//            var patchItems = new List<object>
//            {
//                new PatchItemString { Op = "add", Path = "/Title", Value = e.NewTitle },
//                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
//                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
//            };
//
//            PatchBook(patchItems, e.BookKey.ToString());
//            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
//        }
//
//        public void OnBookSubTitleChanged(object sender, BookSubTitleChangedEventArgs e)
//        {
//            var patchItems = new List<object>
//            {
//                new PatchItemString { Op = "add", Path = "/SubTitle", Value = e.NewSubTitle },
//                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
//                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
//            };
//
//            PatchBook(patchItems, e.BookKey.ToString());
//            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
//        }
//
//        public void OnBookSeriesTitleChanged(object sender, BookSeriesTitleChangedEventArgs e)
//        {
//            var patchItems = new List<object>
//            {
//                new PatchItemString { Op = "add", Path = "/SeriesTitle", Value = e.NewSeriesTitle },
//                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
//                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
//            };
//
//            PatchBook(patchItems, e.BookKey.ToString());
//            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
//        }
//
//        public void OnBookSeriesVolumeChanged(object sender, BookSeriesVolumeChangedEventArgs e)
//        {
//            var patchItems = new List<object>
//            {
//                new PatchItemString { Op = "add", Path = "/SeriesVolume", Value = e.NewSeriesVolume },
//                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
//                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
//            };
//
//            PatchBook(patchItems, e.BookKey.ToString());
//            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
//        }
//
//        public void OnBookGenreChanged(object sender, BookGenreChangedEventArgs e)
//        {
//            var patchItems = new List<object>
//            {
//                new PatchItemObject { Op = "add", Path = "/Genre", Value = e.NewGenre },
//                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
//                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
//            };
//
//            PatchBook(patchItems, e.BookKey.ToString());
//            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
//        }
//
//        public void OnBookLanguageChanged(object sender, BookLanguageChangedEventArgs e)
//        {
//            var patchItems = new List<object>
//            {
//                new PatchItemObject { Op = "add", Path = "/Language", Value = e.NewLanguage },
//                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
//                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
//            };
//
//            PatchBook(patchItems, e.BookKey.ToString());
//            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
//        }
//
//        public void OnBookSynopsisUpdated(object sender, BookSynopsisUpdatedEventArgs e)
//        {
//            var patchItems = new List<object>
//            {
//                new PatchItemString { Op = "add", Path = "/Synopsis", Value = e.NewSynopsis },
//                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
//                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
//            };
//
//            PatchBook(patchItems, e.BookKey.ToString());
//            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
//        }
//
//        public void OnBookCompleted(object sender, BookCompletedEventArgs e)
//        {
//            var patchItems = new List<object>
//            {
//                new PatchItemObject { Op = "add", Path = "/Completed", Value = true },
//                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
//                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
//            };
//
//            PatchBook(patchItems, e.BookKey.ToString());
//            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
//        }
//
//        public void OnBookSetAsInProgress(object sender, BookSetAsInProgressEventArgs e)
//        {
//            var patchItems = new List<object>
//            {
//                new PatchItemObject { Op = "add", Path = "/Completed", Value = false },
//                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
//                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
//            };
//
//            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
//            var json = JsonConvert.SerializeObject(patchItems, settings);
//
//            _orchestrate.Patch(Definitions.BookCollection, e.BookKey.ToString(), json);
//            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
//        }
//
//        public void OnBookAbandoned(object sender, BookAbandonedEventArgs e)
//        {
//            var patchItems = new List<object>
//            {
//                new PatchItemObject { Op = "add", Path = "/Abandoned", Value = true },
//                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
//                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
//            };
//
//            PatchBook(patchItems, e.BookKey.ToString());
//            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
//        }
//
//        public void OnBookRevived(object sender, BookRevivedEventArgs e)
//        {
//            var patchItems = new List<object>
//            {
//                new PatchItemObject { Op = "add", Path = "/Abandoned", Value = false },
//                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
//                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
//            };
//
//            PatchBook(patchItems, e.BookKey.ToString());
//            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
//        }
//
//        public void OnBookPublished(object sender, BookPublishedEventArgs e)
//        {
//            var patchItems = new List<object>
//            {
//                new PatchItemObject { Op = "add", Path = "/Published", Value = true },
//                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
//                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
//            };
//
//            PatchBook(patchItems, e.BookKey.ToString());
//            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
//        }

    }
}