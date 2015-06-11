using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Krakkl.Persistence.Authorship
{
    internal class ProcessAuthorshipMessage
    {
        private readonly Orchestrate.Net.Orchestrate _orchestrate;

        public ProcessAuthorshipMessage()
        {
            var configuration = new Configuration().AddJsonFile("config.json");
            _orchestrate = new Orchestrate.Net.Orchestrate(configuration["Data:Orchestrate:ApiKey"]);
        }

        public void ProcessEvent(CloudQueueMessage message)
        {
            var eventModel = JsonConvert.DeserializeObject<AuthorshipEventModel>(message.AsString);
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var json = JsonConvert.SerializeObject(eventModel, settings);

            _orchestrate.Put(Definitions.BookEventsCollection, message.Id, json);
        }

        internal async Task ProcessMessageAsync(CloudQueueMessage message)
        {
            var eventModel = JsonConvert.DeserializeObject<AuthorshipEventModel>(message.AsString);

            switch (eventModel.EventType)
            {
                case "BookCreated":
                    await BookCreatedHandler(eventModel);
                    break;

                case "AuthorAddedToBook":
                case "AuthorRemovedFromBook":
                    await AuthorChangedHandler(eventModel);
                    break;

                case "BookRetitled":
                    await BookRetitledHandler(eventModel);
                    break;

                case "BookSubTitleChanged":
                    await BookSubTitleChangedHandler(eventModel);
                    break;

                case "BookSeriesTitleChanged":
                    await BookSeriesTitleChangedHandler(eventModel);
                    break;

                case "BookSeriesVolumeChanged":
                    await BookSeriesVolumeChangedHandler(eventModel);
                    break;

                case "BookGenreChanged":
                    await BookGenreChangedHandler(eventModel);
                    break;

                case "BookLanguageChanged":
                    await BookLanguageChangedHandler(eventModel);
                    break;

                case "BookSynopsisUpdated":
                    await BookSynopsisUpdatedHandler(eventModel);
                    break;

                case "BookCompleted":
                    await BookCompletedHandler(eventModel);
                    break;

                case "BookSetAsInProgress":
                    await BookSetAsInProgressHandler(eventModel);
                    break;

                case "BookAbandoned":
                    await BookAbandonedHandler(eventModel);
                    break;

                case "BookRevived":
                    await BookRevivedHandler(eventModel);
                    break;

                case "BookPublished":
                    await BookPublishedHanlder(eventModel);
                    break;
            }
        }

        private async Task BookCreatedHandler(AuthorshipEventModel eventModel)
        {
            var bookKey = eventModel.BookKey.ToString();
            var language = new LanguageModel { Key = eventModel.LanguageKey, Name = eventModel.LanguageName };

            var state = new BookState
            {
                Key = eventModel.BookKey,
//                Authors = eventModel.ValidAuthors,
                Language = language,
                CreatedAt = eventModel.CreatedAt.GetValueOrDefault(),
                CreatedBy = eventModel.AddedAuthor.Key
            };

            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            var json = JsonConvert.SerializeObject(state, settings);
            Console.WriteLine(DateTime.UtcNow.ToLongTimeString() + " Put Started - " + eventModel.EventType);
            await _orchestrate.PutAsync(Definitions.BookCollection, bookKey, json);
            Console.WriteLine(DateTime.UtcNow.ToLongTimeString() + " Put Complete - " + eventModel.EventType);

            var eventJson = JsonConvert.SerializeObject(eventModel, settings);
            await _orchestrate.PutEventAsync(Definitions.BookCollection, bookKey, "update", eventModel.UpdatedAt, eventJson);
            Console.WriteLine(DateTime.UtcNow.ToLongTimeString() + " Event - " + eventModel.EventType);
        }

        private async Task AuthorChangedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
//                new PatchItemObject { Op = "replace", Path = "/Authors", Value = eventModel.ValidAuthors },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault() }
            };

            await PatchBook(patchItems, eventModel);
        }

        private async Task BookRetitledHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemString { Op = "add", Path = "/Title", Value = eventModel.NewTitle },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault() }
            };

            await PatchBook(patchItems, eventModel);
        }

        private async Task BookSubTitleChangedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemString { Op = "add", Path = "/SubTitle", Value = eventModel.NewSubTitle },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault() }
            };

            await PatchBook(patchItems, eventModel);
        }

        private async Task BookSeriesTitleChangedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemString { Op = "add", Path = "/SeriesTitle", Value = eventModel.NewSeriesTitle },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault() }
            };

            await PatchBook(patchItems, eventModel);
        }

        private async Task BookSeriesVolumeChangedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemString { Op = "add", Path = "/SeriesVolume", Value = eventModel.NewSeriesVolume },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault() }
            };

            await PatchBook(patchItems, eventModel);
        }

        private async Task BookGenreChangedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Genre", Value = eventModel.NewGenre },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault() }
            };

            await PatchBook(patchItems, eventModel);
        }

        private async Task BookLanguageChangedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Language", Value = eventModel.NewLanguage },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault() }
            };

            await PatchBook(patchItems, eventModel);
        }

        private async Task BookSynopsisUpdatedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemString { Op = "add", Path = "/Synopsis", Value = eventModel.NewSynopsis },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault() }
            };

            await PatchBook(patchItems, eventModel);
        }

        private async Task BookCompletedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Completed", Value = true },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault() }
            };

            await PatchBook(patchItems, eventModel);
        }

        private async Task BookSetAsInProgressHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Completed", Value = false },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault() }
            };

            await PatchBook(patchItems, eventModel);
        }

        private async Task BookAbandonedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Abandoned", Value = true },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault() }
            };

            await PatchBook(patchItems, eventModel);
        }

        private async Task BookRevivedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Abandoned", Value = false },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault() }
            };

            await PatchBook(patchItems, eventModel);
        }

        private async Task BookPublishedHanlder(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Published", Value = true },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault() }
            };

            await PatchBook(patchItems, eventModel);
        }

        private async Task PatchBook(List<object> patchItems, AuthorshipEventModel eventModel)
        {
            var bookKey = eventModel.BookKey.ToString();
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var json = JsonConvert.SerializeObject(patchItems, settings);

            Console.WriteLine(DateTime.UtcNow.ToLongTimeString() + " Patch Started - " + eventModel.EventType);
            await _orchestrate.PatchAsync(Definitions.BookCollection, bookKey, json);
            Console.WriteLine(DateTime.UtcNow.ToLongTimeString() + " Patch Complete - " + eventModel.EventType);

            var eventJson = JsonConvert.SerializeObject(eventModel, settings);
            await _orchestrate.PutEventAsync(Definitions.BookCollection, bookKey, "update", eventModel.UpdatedAt, eventJson);

            Console.WriteLine(DateTime.UtcNow.ToLongTimeString() + " Event - " + eventModel.EventType);
        }
    }
}