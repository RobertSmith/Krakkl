using System.Collections.Generic;
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

        internal void ProcessMessage(CloudQueueMessage message)
        {
            var eventModel = JsonConvert.DeserializeObject<AuthorshipEventModel>(message.AsString);

            switch (eventModel.EventType)
            {
                case "BookCreated":
                    BookCreatedHandler(eventModel);
                    break;

                case "AuthorAddedToBook":
                case "AuthorRemovedFromBook":
                    AuthorChangedHandler(eventModel);
                    break;

                case "BookRetitled":
                    BookRetitledHandler(eventModel);
                    break;

                case "BookSubTitleChanged":
                    BookSubTitleChangedHandler(eventModel);
                    break;

                case "BookSeriesTitleChanged":
                    BookSeriesTitleChangedHandler(eventModel);
                    break;

                case "BookSeriesVolumeChanged":
                    BookSeriesVolumeChangedHandler(eventModel);
                    break;

                case "BookGenreChanged":
                    BookGenreChangedHandler(eventModel);
                    break;

                case "BookLanguageChanged":
                    BookLanguageChangedHandler(eventModel);
                    break;

                case "BookSynopsisUpdated":
                    BookSynopsisUpdatedHandler(eventModel);
                    break;

                case "BookCompleted":
                    BookCompletedHandler(eventModel);
                    break;

                case "BookSetAsInProgress":
                    BookSetAsInProgressHandler(eventModel);
                    break;

                case "BookAbandoned":
                    BookAbandonedHandler(eventModel);
                    break;

                case "BookRevived":
                    BookRevivedHandler(eventModel);
                    break;

                case "BookPublished":
                    BookPublishedHanlder(eventModel);
                    break;
            }
        }

        private void BookCreatedHandler(AuthorshipEventModel eventModel)
        {
            var bookKey = eventModel.BookKey.ToString();
            var language = new LanguageModel { Key = eventModel.LanguageKey, Name = eventModel.LanguageName };

            var state = new BookState
            {
                Key = eventModel.BookKey,
                Authors = eventModel.ValidAuthors,
                Language = language,
                CreatedAt = eventModel.CreatedAt,
                CreatedBy = eventModel.AddedAuthor.Key
            };

            _orchestrate.Put(Definitions.BookCollection, bookKey, state);
            _orchestrate.PutEvent(Definitions.BookCollection, bookKey, "update", eventModel.CreatedAt, eventModel);
        }

        private void AuthorChangedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "replace", Path = "/Authors", Value = eventModel.ValidAuthors },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt }
            };

            PatchBook(patchItems, eventModel);
        }

        private void BookRetitledHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemString { Op = "add", Path = "/Title", Value = eventModel.NewTitle },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt }
            };

            PatchBook(patchItems, eventModel);
        }

        private void BookSubTitleChangedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemString { Op = "add", Path = "/SubTitle", Value = eventModel.NewSubTitle },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt }
            };

            PatchBook(patchItems, eventModel);
        }

        private void BookSeriesTitleChangedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemString { Op = "add", Path = "/SeriesTitle", Value = eventModel.NewSeriesTitle },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt }
            };

            PatchBook(patchItems, eventModel);
        }

        private void BookSeriesVolumeChangedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemString { Op = "add", Path = "/SeriesVolume", Value = eventModel.NewSeriesVolume },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt }
            };

            PatchBook(patchItems, eventModel);
        }

        private void BookGenreChangedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Genre", Value = eventModel.NewGenre },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt }
            };

            PatchBook(patchItems, eventModel);
        }

        private void BookLanguageChangedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Language", Value = eventModel.NewLanguage },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt }
            };

            PatchBook(patchItems, eventModel);
        }

        private void BookSynopsisUpdatedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemString { Op = "add", Path = "/Synopsis", Value = eventModel.NewSynopsis },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt }
            };

            PatchBook(patchItems, eventModel);
        }

        private void BookCompletedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Completed", Value = true },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt }
            };

            PatchBook(patchItems, eventModel);
        }

        private void BookSetAsInProgressHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Completed", Value = false },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt }
            };

            PatchBook(patchItems, eventModel);
        }

        private void BookAbandonedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Abandoned", Value = true },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt }
            };

            PatchBook(patchItems, eventModel);
        }

        private void BookRevivedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Abandoned", Value = false },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt }
            };

            PatchBook(patchItems, eventModel);
        }

        private void BookPublishedHanlder(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Published", Value = true },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt }
            };

            PatchBook(patchItems, eventModel);
        }

        private void PatchBook(List<object> patchItems, AuthorshipEventModel eventModel)
        {
            var bookKey = eventModel.BookKey.ToString();
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var json = JsonConvert.SerializeObject(patchItems, settings);
        
            _orchestrate.Patch(Definitions.BookCollection, bookKey, json);
            _orchestrate.PutEvent(Definitions.BookCollection, bookKey, "update", eventModel.UpdatedAt, eventModel);
        }
    }
}