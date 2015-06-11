using System;
using System.Collections.Generic;
using System.Linq;
using Krakkl.Authorship.Models;
using Krakkl.Authorship.Repository;
using Newtonsoft.Json;

namespace Krakkl.Authorship.Core
{
    // all event arguments should inherit from EventArgs
    internal class BookEventArgs : EventArgs
    {
        public Guid BookKey { get; private set; }
        public DateTime TimeStamp => DateTime.UtcNow;
        public string EventSource => "Book.Authorship.Krakkl";

        public BookEventArgs(BookState state)
        {
            BookKey = state.Key;
        }

        public override string ToString()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, settings);
        }
    }

    internal sealed class BookCreatedEventArgs : BookEventArgs
    {
        public AuthorModel AddedAuthor { get; private set; }
        public List<AuthorModel> ValidAuthors { get; private set; }
        public string LanguageKey { get; private set; }
        public string LanguageName { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid CreatedBy { get; private set; }
        public string EventType => "BookCreated";

        public BookCreatedEventArgs(BookState state) : base(state)
        {
            AddedAuthor = state.Authors.First();
            ValidAuthors = state.Authors;
            LanguageKey = state.Language?.Key;
            LanguageName = state.Language?.Name;
            CreatedAt = state.CreatedAt;
            CreatedBy = state.CreatedBy;
        }
    }

    internal sealed class AuthorAddedToBookEventArgs : BookEventArgs
    {
        public AuthorModel AddedAuthor { get; private set; }
        public List<AuthorModel> ValidAuthors { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "AuthorAddedToBook";

        public AuthorAddedToBookEventArgs(BookState state, AuthorModel addedAuthor) : base(state)
        {
            AddedAuthor = addedAuthor;
            ValidAuthors = state.Authors;
            UpdatedAt = state.UpdatedAt.GetValueOrDefault();
            UpdatedBy = state.UpdatedBy.GetValueOrDefault();
        }
    }

    internal sealed class AuthorRemovedFromBookEventArgs : BookEventArgs
    {
        public AuthorModel RemovedAuthor { get; private set; }
        public List<AuthorModel> ValidAuthors { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "AuthorRemovedFromBook";

        public AuthorRemovedFromBookEventArgs(BookState state, AuthorModel removedAuthor) : base(state)
        {
            RemovedAuthor = removedAuthor;
            ValidAuthors = state.Authors;
            UpdatedAt = state.UpdatedAt.GetValueOrDefault();
            UpdatedBy = state.UpdatedBy.GetValueOrDefault();
        }
    }

    internal sealed class BookRetitledEventArgs : BookEventArgs
    {
        public string NewTitle { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookRetitled";

        public BookRetitledEventArgs(BookState state) : base(state)
        {
            NewTitle = state.Title;
            UpdatedAt = state.UpdatedAt.GetValueOrDefault();
            UpdatedBy = state.UpdatedBy.GetValueOrDefault();
        }
    }

    internal sealed class BookSubTitleChangedEventArgs : BookEventArgs
    {
        public string NewSubTitle { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookSubTitleChanged";

        public BookSubTitleChangedEventArgs(BookState state) : base(state)
        {
            NewSubTitle = state.SubTitle;
            UpdatedAt = state.UpdatedAt.GetValueOrDefault();
            UpdatedBy = state.UpdatedBy.GetValueOrDefault();
        }
    }

    internal sealed class BookSeriesTitleChangedEventArgs : BookEventArgs
    {
        public string NewSeriesTitle { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookSeriesTitleChanged";

        public BookSeriesTitleChangedEventArgs(BookState state) : base(state)
        {
            NewSeriesTitle = state.SeriesTitle;
            UpdatedAt = state.UpdatedAt.GetValueOrDefault();
            UpdatedBy = state.UpdatedBy.GetValueOrDefault();
        }
    }

    internal sealed class BookSeriesVolumeChangedEventArgs : BookEventArgs
    {
        public string NewSeriesVolume { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookSeriesVolumeChanged";

        public BookSeriesVolumeChangedEventArgs(BookState state) : base(state)
        {
            NewSeriesVolume = state.SeriesTitle;
            UpdatedAt = state.UpdatedAt.GetValueOrDefault();
            UpdatedBy = state.UpdatedBy.GetValueOrDefault();
        }
    }

    internal sealed class BookGenreChangedEventArgs : BookEventArgs
    {
        public GenreModel NewGenre { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookGenreChanged";

        public BookGenreChangedEventArgs(BookState state) : base(state)
        {
            NewGenre = state.Genre;
            UpdatedAt = state.UpdatedAt.GetValueOrDefault();
            UpdatedBy = state.UpdatedBy.GetValueOrDefault();
        }
    }

    internal sealed class BookLanguageChangedEventArgs : BookEventArgs
    {
        public LanguageModel NewLanguage { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookLanguageChanged";

        public BookLanguageChangedEventArgs(BookState state) : base(state)
        {
            NewLanguage = state.Language;
            UpdatedAt = state.UpdatedAt.GetValueOrDefault();
            UpdatedBy = state.UpdatedBy.GetValueOrDefault();
        }
    }

    internal sealed class BookSynopsisUpdatedEventArgs : BookEventArgs
    {
        public string NewSynopsis { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookSynopsisUpdated";

        public BookSynopsisUpdatedEventArgs(BookState state) : base(state)
        {
            NewSynopsis = state.SeriesTitle;
            UpdatedAt = state.UpdatedAt.GetValueOrDefault();
            UpdatedBy = state.UpdatedBy.GetValueOrDefault();
        }
    }

    internal sealed class BookCompletedEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookCompleted";

        public BookCompletedEventArgs(BookState state) : base(state)
        {
            UpdatedAt = state.UpdatedAt.GetValueOrDefault();
            UpdatedBy = state.UpdatedBy.GetValueOrDefault();
        }
    }

    internal sealed class BookSetAsInProgressEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookSetAsInProgress";

        public BookSetAsInProgressEventArgs(BookState state) : base(state)
        {
            UpdatedAt = state.UpdatedAt.GetValueOrDefault();
            UpdatedBy = state.UpdatedBy.GetValueOrDefault();
        }
    }

    internal sealed class BookAbandonedEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookAbandoned";

        public BookAbandonedEventArgs(BookState state) : base(state)
        {
            UpdatedAt = state.UpdatedAt.GetValueOrDefault();
            UpdatedBy = state.UpdatedBy.GetValueOrDefault();
        }
    }

    internal sealed class BookRevivedEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookRevived";

        public BookRevivedEventArgs(BookState state) : base(state)
        {
            UpdatedAt = state.UpdatedAt.GetValueOrDefault();
            UpdatedBy = state.UpdatedBy.GetValueOrDefault();
        }
    }

    internal sealed class BookPublishedEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookPublished";

        public BookPublishedEventArgs(BookState state) : base(state)
        {
            UpdatedAt = state.UpdatedAt.GetValueOrDefault();
            UpdatedBy = state.UpdatedBy.GetValueOrDefault();
        }
    }
}