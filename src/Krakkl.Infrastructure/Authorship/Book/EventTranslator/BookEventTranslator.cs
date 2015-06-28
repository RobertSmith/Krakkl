using System;
using System.Collections.Generic;
using System.Threading;
using Krakkl.Infrastructure.OrchestrateModels;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Krakkl.Infrastructure.Authorship.Book.EventTranslator
{
    public class BookEventTranslator
    {
        private readonly Orchestrate.Net.Orchestrate _orchestrate;
        private readonly CloudQueue _queue;

        public BookEventTranslator()
        {
            var configuration = new Configuration().AddJsonFile("config.json");
            var storageConnectionString = configuration["Data:AzureStorage:ConnectionString"];
            _orchestrate = new Orchestrate.Net.Orchestrate(configuration["Data:Orchestrate:ApiKey"]);

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference("authorship");

            Run();
        }

        private async void Run()
        {
            var maxRetries = 5;
            var tryCount = 0;

            await _queue.CreateIfNotExistsAsync();

            while (true)
            {
                var message = await _queue.GetMessageAsync();

                if (message != null)
                {
                    try
                    {
                        ProcessEvent(message);
                        await _queue.DeleteMessageAsync(message);
                        tryCount = 0;
                    }
                    catch (Exception)
                    {
                        tryCount++;

                        if (tryCount >= maxRetries)
                        {
                            tryCount = 0;
                            await _queue.DeleteMessageAsync(message);
                        }
                    }
                }
                else
                    Thread.Sleep(1000);
            }

            // ReSharper disable once FunctionNeverReturns
        }

        private void ProcessEvent(CloudQueueMessage message)
        {
            var bookEvent = JsonConvert.DeserializeObject<BookEventModel>(message.AsString);

            try
            {
                switch (bookEvent.EventType)
                {
                    case "BookCreated":
                        BookCreatedHandler(bookEvent);
                        break;

                    case "AuthorAddedToBook":
                        AuthorAddedHandler(bookEvent);
                        break;

                    case "AuthorRemovedFromBook":
                        AuthorRemovedHandler(bookEvent);
                        break;

                    case "BookRetitled":
                        BookRetitledHandler(bookEvent);
                        break;

                    case "BookSubTitleChanged":
                        BookSubTitleChangedHandler(bookEvent);
                        break;

                    case "BookSeriesTitleChanged":
                        BookSeriesTitleChangedHandler(bookEvent);
                        break;

                    case "BookSeriesVolumeChanged":
                        BookSeriesVolumeChangedHandler(bookEvent);
                        break;

                    case "BookGenreChanged":
                        BookGenreChangedHandler(bookEvent);
                        break;

                    case "BookLanguageChanged":
                        BookLanguageChangedHandler(bookEvent);
                        break;

                    case "BookSynopsisUpdated":
                        BookSynopsisUpdatedHandler(bookEvent);
                        break;

                    case "BookCompleted":
                        BookCompletedHandler(bookEvent);
                        break;

                    case "BookSetAsInProgress":
                        BookSetAsInProgressHandler(bookEvent);
                        break;

                    case "BookAbandoned":
                        BookAbandonedHandler(bookEvent);
                        break;

                    case "BookRevived":
                        BookRevivedHandler(bookEvent);
                        break;

                    case "BookPublished":
                        BookPublishedHanlder(bookEvent);
                        break;

                    case "BookCoverArtSet":
                        BookCoverArtSet(bookEvent);
                        break;
                }
            }
            catch (AggregateException ex)
            {
                var failedTranslationMessage = new FailedTranslationMessage
                {
                    EventData = bookEvent.ToString(),
                    Exception = ex.ToString()
                };

                _orchestrate.Post(Definitions.TranslationFailureCollection, failedTranslationMessage);

                throw;
            }
        }

        private void BookCreatedHandler(BookEventModel eventModel)
        {
            var bookKey = eventModel.BookKey.ToString();

            var state = new NewBookModel
            {
                Key = eventModel.BookKey,
                Language = eventModel.Language,
                CreatedAt = eventModel.CreatedAt.GetValueOrDefault(),
                CreatedBy = eventModel.AddedAuthor.Key
            };

            state.Authors.Add(eventModel.AddedAuthor);

            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            var json = JsonConvert.SerializeObject(state, settings);

            _orchestrate.Put(Definitions.BookCollection, bookKey, json);
        }

        private void AuthorAddedHandler(BookEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject {Op = "append", Path = "/Authors", Value = eventModel.AddedAuthor},
                new PatchItemString {Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString()},
                new PatchItemDate {Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault()},
                new PatchItemDate {Op ="add", Path="/LastEventTimeStamp", Value = eventModel.TimeStamp }
            };

            PatchBook(patchItems, eventModel.BookKey.ToString());
        }

        private void AuthorRemovedHandler(BookEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject {Op = "replace", Path = "/Authors", Value = eventModel.ValidAuthors},
                new PatchItemString {Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString()},
                new PatchItemDate {Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault()},
                new PatchItemDate {Op ="add", Path="/LastEventTimeStamp", Value = eventModel.TimeStamp }
            };

            PatchBook(patchItems, eventModel.BookKey.ToString());
        }

        private void BookRetitledHandler(BookEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemString {Op = "add", Path = "/Title", Value = eventModel.NewTitle},
                new PatchItemString {Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString()},
                new PatchItemDate {Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault()},
                new PatchItemDate {Op ="add", Path="/LastEventTimeStamp", Value = eventModel.TimeStamp }
            };

            PatchBook(patchItems, eventModel.BookKey.ToString());
        }

        private void BookSubTitleChangedHandler(BookEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemString {Op = "add", Path = "/SubTitle", Value = eventModel.NewSubTitle},
                new PatchItemString {Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString()},
                new PatchItemDate {Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault()},
                new PatchItemDate {Op ="add", Path="/LastEventTimeStamp", Value = eventModel.TimeStamp }
            };

            PatchBook(patchItems, eventModel.BookKey.ToString());
        }

        private void BookSeriesTitleChangedHandler(BookEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemString {Op = "add", Path = "/SeriesTitle", Value = eventModel.NewSeriesTitle},
                new PatchItemString {Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString()},
                new PatchItemDate {Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault()},
                new PatchItemDate {Op = "add", Path = "/LastEventTimeStamp", Value = eventModel.TimeStamp}
            };

            PatchBook(patchItems, eventModel.BookKey.ToString());
        }

        private void BookSeriesVolumeChangedHandler(BookEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemString {Op = "add", Path = "/SeriesVolume", Value = eventModel.NewSeriesVolume},
                new PatchItemString {Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString()},
                new PatchItemDate {Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault()},
                new PatchItemDate {Op ="add", Path="/LastEventTimeStamp", Value = eventModel.TimeStamp }
            };

            PatchBook(patchItems, eventModel.BookKey.ToString());
        }

        private void BookGenreChangedHandler(BookEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject {Op = "add", Path = "/Genre", Value = eventModel.NewGenre},
                new PatchItemString {Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString()},
                new PatchItemDate {Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault()},
                new PatchItemDate {Op ="add", Path="/LastEventTimeStamp", Value = eventModel.TimeStamp }
            };

            PatchBook(patchItems, eventModel.BookKey.ToString());
        }

        private void BookLanguageChangedHandler(BookEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject {Op = "add", Path = "/Language", Value = eventModel.NewLanguage},
                new PatchItemString {Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString()},
                new PatchItemDate {Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault()},
                new PatchItemDate {Op ="add", Path="/LastEventTimeStamp", Value = eventModel.TimeStamp }
            };

            PatchBook(patchItems, eventModel.BookKey.ToString());
        }

        private void BookSynopsisUpdatedHandler(BookEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemString {Op = "add", Path = "/Synopsis", Value = eventModel.NewSynopsis},
                new PatchItemString {Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString()},
                new PatchItemDate {Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault()},
                new PatchItemDate {Op ="add", Path="/LastEventTimeStamp", Value = eventModel.TimeStamp }
            };

            PatchBook(patchItems, eventModel.BookKey.ToString());
        }

        private void BookCompletedHandler(BookEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject {Op = "add", Path = "/Completed", Value = true},
                new PatchItemString {Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString()},
                new PatchItemDate {Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault()},
                new PatchItemDate {Op ="add", Path="/LastEventTimeStamp", Value = eventModel.TimeStamp }
            };

            PatchBook(patchItems, eventModel.BookKey.ToString());
        }

        private void BookSetAsInProgressHandler(BookEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject {Op = "add", Path = "/Completed", Value = false},
                new PatchItemString {Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString()},
                new PatchItemDate {Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault()},
                new PatchItemDate {Op ="add", Path="/LastEventTimeStamp", Value = eventModel.TimeStamp }
            };

            PatchBook(patchItems, eventModel.BookKey.ToString());
        }

        private void BookAbandonedHandler(BookEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject {Op = "add", Path = "/Abandoned", Value = true},
                new PatchItemString {Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString()},
                new PatchItemDate {Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault()},
                new PatchItemDate {Op ="add", Path="/LastEventTimeStamp", Value = eventModel.TimeStamp }
            };

            PatchBook(patchItems, eventModel.BookKey.ToString());
        }

        private void BookRevivedHandler(BookEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject {Op = "add", Path = "/Abandoned", Value = false},
                new PatchItemString {Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString()},
                new PatchItemDate {Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault()},
                new PatchItemDate {Op ="add", Path="/LastEventTimeStamp", Value = eventModel.TimeStamp }
            };

            PatchBook(patchItems, eventModel.BookKey.ToString());
        }

        private void BookPublishedHanlder(BookEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject {Op = "add", Path = "/Published", Value = true},
                new PatchItemString {Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString()},
                new PatchItemDate {Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault()},
                new PatchItemDate {Op ="add", Path="/LastEventTimeStamp", Value = eventModel.TimeStamp }
            };

            PatchBook(patchItems, eventModel.BookKey.ToString());
        }

        private void BookCoverArtSet(BookEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject {Op = "add", Path = "/CoverArtKey", Value = eventModel.CoverArtKey},
                new PatchItemString {Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString()},
                new PatchItemDate {Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault()},
                new PatchItemDate {Op ="add", Path="/LastEventTimeStamp", Value = eventModel.TimeStamp }
            };

            PatchBook(patchItems, eventModel.BookKey.ToString());
        }

        private void PatchBook(List<object> patchItems, string bookKey)
        {
            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            var json = JsonConvert.SerializeObject(patchItems, settings);

            _orchestrate.Patch(Definitions.BookCollection, bookKey, json);
        }
    }
}