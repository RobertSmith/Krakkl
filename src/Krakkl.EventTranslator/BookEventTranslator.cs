using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Framework.ConfigurationModel;
using Newtonsoft.Json;
using Orchestrate.Net;

namespace Krakkl.EventTranslator
{
    public class BookEventTranslator
    {
        private readonly Orchestrate.Net.Orchestrate _orchestrate;
        private int _idleCount;

        public BookEventTranslator()
        {
            var configuration = new Configuration().AddJsonFile("config.json");
            _orchestrate = new Orchestrate.Net.Orchestrate(configuration["Data:Orchestrate:ApiKey"]);

            Task.Run(() => Run());
        }

        private void Run()
        {
            while (true)
            {
                if (_idleCount > 30)
                    _idleCount = 30;

                Thread.Sleep(_idleCount * 1000);

                var newBookEvents = _orchestrate.Search("BookEvents", "TranslatedToQueryStorage = false", 50, 0, "TimeStamp:asc");

                if (newBookEvents?.Count == 0)
                {
                    _idleCount++;
                    continue;
                }

                _idleCount = 0;

                foreach (var newBookEvent in newBookEvents.Results)
                {
                    var bookEvent = JsonConvert.DeserializeObject<BookEventModel>(newBookEvent.Value.ToString());

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
                        }

                        UpdateEventAsTranslated(newBookEvent);
                    }
                    catch (AggregateException ex)
                    {
                        var patchItems = new List<object>();

                        if (bookEvent.TranslationAttemptCount >= 10)
                        {
                            patchItems.Add(new PatchItemString { Op = "add", Path = "/TranslationFailureMessage", Value = ex.InnerException.Message });
                            patchItems.Add(new PatchItemObject { Op = "add", Path = "/TranslatedToQueryStorage", Value = true });
                        }
                        else
                            patchItems.Add(new PatchItemInt { Op = "add", Path = "/TranslationAttemptCount", Value = ++bookEvent.TranslationAttemptCount });

                        var bookEventKey = newBookEvent.Path.Key;
                        var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                        var json = JsonConvert.SerializeObject(patchItems, settings);

                        _orchestrate.Patch(Definitions.BookEventsCollection, bookEventKey, json);

                        // wait one sec before processing next batch
                        _idleCount = 1;
                    }
                }
            }
        }

        private void BookCreatedHandler(BookEventModel eventModel)
        {
            var bookKey = eventModel.BookKey.ToString();
            var language = new LanguageModel {Key = eventModel.LanguageKey, Name = eventModel.LanguageName};

            var state = new NewBookModel
            {
                Key = eventModel.BookKey,
                Language = language,
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

            PatchBook(patchItems, eventModel);
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

            PatchBook(patchItems, eventModel);
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

            PatchBook(patchItems, eventModel);
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

            PatchBook(patchItems, eventModel);
        }

        private void BookSeriesTitleChangedHandler(BookEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemString {Op = "add", Path = "/SeriesTitle", Value = eventModel.NewSeriesTitle},
                new PatchItemString {Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString()},
                new PatchItemDate {Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt.GetValueOrDefault()},
                new PatchItemDate {Op ="add", Path="/LastEventTimeStamp", Value = eventModel.TimeStamp }
            };

            PatchBook(patchItems, eventModel);
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

            PatchBook(patchItems, eventModel);
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

            PatchBook(patchItems, eventModel);
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

            PatchBook(patchItems, eventModel);
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

            PatchBook(patchItems, eventModel);
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

            PatchBook(patchItems, eventModel);
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

            PatchBook(patchItems, eventModel);
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

            PatchBook(patchItems, eventModel);
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

            PatchBook(patchItems, eventModel);
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

            PatchBook(patchItems, eventModel);
        }

        private void PatchBook(List<object> patchItems, BookEventModel eventModel)
        {
            var bookKey = eventModel.BookKey.ToString();
            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            var json = JsonConvert.SerializeObject(patchItems, settings);

            _orchestrate.Patch(Definitions.BookCollection, bookKey, json);
        }

        private void UpdateEventAsTranslated(Result result)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/TranslatedToQueryStorage", Value = true }
            };

            var bookEventKey = result.Path.Key;
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var json = JsonConvert.SerializeObject(patchItems, settings);

            _orchestrate.Patch(Definitions.BookEventsCollection, bookEventKey, json);
        }
    }
}