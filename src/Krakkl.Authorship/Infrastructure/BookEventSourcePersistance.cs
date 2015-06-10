using System.Collections.Generic;
using Krakkl.Authorship.Core;
using Krakkl.Authorship.Models;
using Microsoft.Framework.ConfigurationModel;
using Newtonsoft.Json;

namespace Krakkl.Authorship.Infrastructure
{
    internal class BookEventSourcePersistance
    {
        private readonly Orchestrate.Net.Orchestrate _orchestrate;

        public BookEventSourcePersistance()
        {
            var configuration = new Configuration().AddJsonFile("config.json");

            _orchestrate = new Orchestrate.Net.Orchestrate(configuration["Data:Orchestrate:ApiKey"]);
        }

        public void OnBookCreated(object sender, BookCreatedEventArgs e)
        {
            var author = new AuthorModel(e.AuthorKey, e.AuthorName);
            var language = new LanguageModel(e.LanguageKey, e.LanguageName);

            var state = new BookState
            {
                Key = e.BookKey,
                Language = language,
                CreatedAt = e.CreatedAt,
                CreatedBy = e.AuthorKey,
                Published = false,
                Abandoned = false,
                Completed = false,
                DMCA = false,
                Title = e.Title
            };

            state.Authors.Add(author);

            _orchestrate.Put(Definitions.BookCollection, e.BookKey.ToString(), state);
            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.CreatedAt, e);
        }

        public void OnAuthorAddedToBook(object sender, AuthorAddedToBookEventArgs e)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "replace", Path = "/Authors", Value = e.ValidAuthors },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
            };

            PatchBook(patchItems, e.BookKey.ToString());
            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
        }

        public void OnAuthorRemovedFromBook(object sender, AuthorRemovedFromBookEventArgs e)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "replace", Path = "/Authors", Value = e.ValidAuthors },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
            };

            PatchBook(patchItems, e.BookKey.ToString());
            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
        }

        public void OnBookRetitled(object sender, BookRetitledEventArgs e)
        {
            var patchItems = new List<object>
            {
                new PatchItemString { Op = "add", Path = "/Title", Value = e.NewTitle },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
            };

            PatchBook(patchItems, e.BookKey.ToString());
            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
        }

        public void OnBookSubTitleChanged(object sender, BookSubTitleChangedEventArgs e)
        {
            var patchItems = new List<object>
            {
                new PatchItemString { Op = "add", Path = "/SubTitle", Value = e.NewSubTitle },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
            };

            PatchBook(patchItems, e.BookKey.ToString());
            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
        }

        public void OnBookSeriesTitleChanged(object sender, BookSeriesTitleChangedEventArgs e)
        {
            var patchItems = new List<object>
            {
                new PatchItemString { Op = "add", Path = "/SeriesTitle", Value = e.NewSeriesTitle },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
            };

            PatchBook(patchItems, e.BookKey.ToString());
            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
        }

        public void OnBookSeriesVolumeChanged(object sender, BookSeriesVolumeChangedEventArgs e)
        {
            var patchItems = new List<object>
            {
                new PatchItemString { Op = "add", Path = "/SeriesVolume", Value = e.NewSeriesVolume },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
            };

            PatchBook(patchItems, e.BookKey.ToString());
            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
        }

        public void OnBookGenreChanged(object sender, BookGenreChangedEventArgs e)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Genre", Value = e.NewGenre },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
            };

            PatchBook(patchItems, e.BookKey.ToString());
            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
        }

        public void OnBookLanguageChanged(object sender, BookLanguageChangedEventArgs e)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Language", Value = e.NewLanguage },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
            };

            PatchBook(patchItems, e.BookKey.ToString());
            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
        }

        public void OnBookSynopsisUpdated(object sender, BookSynopsisUpdatedEventArgs e)
        {
            var patchItems = new List<object>
            {
                new PatchItemString { Op = "add", Path = "/Synopsis", Value = e.NewSynopsis },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
            };

            PatchBook(patchItems, e.BookKey.ToString());
            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
        }

        public void OnBookCompleted(object sender, BookCompletedEventArgs e)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Completed", Value = true },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
            };

            PatchBook(patchItems, e.BookKey.ToString());
            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
        }

        public void OnBookSetAsInProgress(object sender, BookSetAsInProgressEventArgs e)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Completed", Value = false },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
            };

            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var json = JsonConvert.SerializeObject(patchItems, settings);

            _orchestrate.Patch(Definitions.BookCollection, e.BookKey.ToString(), json);
            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
        }

        public void OnBookAbandoned(object sender, BookAbandonedEventArgs e)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Abandoned", Value = true },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
            };

            PatchBook(patchItems, e.BookKey.ToString());
            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
        }

        public void OnBookRevived(object sender, BookRevivedEventArgs e)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Abandoned", Value = false },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
            };

            PatchBook(patchItems, e.BookKey.ToString());
            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
        }

        public void OnBookPublished(object sender, BookPublishedEventArgs e)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "add", Path = "/Published", Value = true },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
            };

            PatchBook(patchItems, e.BookKey.ToString());
            _orchestrate.PutEvent(Definitions.BookCollection, e.BookKey.ToString(), "update", e.UpdatedAt, e);
        }

        private void PatchBook(List<object> patchItems, string bookKey)
        {
            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            var json = JsonConvert.SerializeObject(patchItems, settings);

            _orchestrate.Patch(Definitions.BookCollection, bookKey, json);
        }
    }
}