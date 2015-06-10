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
        }

        public void OnAuthorAddedToBook(object sender, AuthorAddedToBookEventArgs e)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "replace", Path = "/Authors", Value = e.ValidAuthors },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
            };

            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var json = JsonConvert.SerializeObject(patchItems, settings);

            _orchestrate.Patch(Definitions.BookCollection, e.BookKey.ToString(), json);
        }

        public void OnAuthorRemovedFromBook(object sender, AuthorRemovedFromBookEventArgs e)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "replace", Path = "/Authors", Value = e.ValidAuthors },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
            };

            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var json = JsonConvert.SerializeObject(patchItems, settings);

            _orchestrate.Patch(Definitions.BookCollection, e.BookKey.ToString(), json);
        }

        public void OnBookRetitled(object sender, BookRetitledEventArgs e)
        {
            var patchItems = new List<object>
            {
                new PatchItemString { Op = "add", Path = "/Title", Value = e.NewTitle },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = e.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = e.UpdatedAt }
            };

            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var json = JsonConvert.SerializeObject(patchItems, settings);

            _orchestrate.Patch(Definitions.BookCollection, e.BookKey.ToString(), json);
        }
    }
}

//public event EventHandler<BookSubTitleChangedEventArgs> BookSubTitleChanged;
//public event EventHandler<BookSeriesTitleChangedEventArgs> BookSeriesTitleChanged;
//public event EventHandler<BookSeriesVolumeChangedEventArgs> BookSeriesVolumeChanged;
//public event EventHandler<BookGenreChangedEventArgs> BookGenreChanged;
//public event EventHandler<BookLanguageChangedEventArgs> BookLanguageChanged;
//public event EventHandler<SynopsisUpdatedEventArgs> SynopsisUpdated;
//public event EventHandler<BookCompletedEventArgs> BookCompleted;
//public event EventHandler<BookSetAsInProgressEventArgs> BookSetAsInProgress;
//public event EventHandler<BookAbandonedEventArgs> BookAbandoned;
//public event EventHandler<BookRevivedEventArgs> BookRevived;
//public event EventHandler<BookPublishedEventArgs> BookPublished;
