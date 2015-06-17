using System;
using System.Collections.Generic;
using System.Linq;
using Krakkl.Authorship.Models;
using Newtonsoft.Json;

namespace Krakkl.Authorship.Aggregates
{
    // all event arguments should inherit from EventArgs
    internal class BookEventArgs : EventArgs
    {
        public Guid BookKey { get; private set; }
        public DateTime TimeStamp { get; private set; }
        public string EventSource => "Book.Authorship.Krakkl";
        public bool TranslatedToQueryStorage => false;

        public BookEventArgs(BookState state)
        {
            BookKey = state.Key;
            TimeStamp = DateTime.UtcNow;
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
        public string LanguageKey { get; private set; }
        public string LanguageName { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid CreatedBy { get; private set; }
        public string EventType => "BookCreated";

        public BookCreatedEventArgs(BookState state, Guid createdBy) : base(state)
        {
            AddedAuthor = state.Authors.First();
            LanguageKey = state.Language?.Key;
            LanguageName = state.Language?.Name;
            CreatedAt = TimeStamp;
            CreatedBy = createdBy;
        }
    }

    internal sealed class AuthorAddedToBookEventArgs : BookEventArgs
    {
        public AuthorModel AddedAuthor { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "AuthorAddedToBook";

        public AuthorAddedToBookEventArgs(BookState state, AuthorModel addedAuthor, Guid updatedBy) : base(state)
        {
            AddedAuthor = addedAuthor;
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class AuthorRemovedFromBookEventArgs : BookEventArgs
    {
        public AuthorModel RemovedAuthor { get; private set; }
        public List<AuthorModel> ValidAuthors { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "AuthorRemovedFromBook";

        public AuthorRemovedFromBookEventArgs(BookState state, AuthorModel removedAuthor, List<AuthorModel> validAuthors, Guid updatedBy) : base(state)
        {
            RemovedAuthor = removedAuthor;
            ValidAuthors = validAuthors;
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookRetitledEventArgs : BookEventArgs
    {
        public string NewTitle { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookRetitled";

        public BookRetitledEventArgs(BookState state, Guid updatedBy) : base(state)
        {
            NewTitle = state.Title;
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookSubTitleChangedEventArgs : BookEventArgs
    {
        public string NewSubTitle { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookSubTitleChanged";

        public BookSubTitleChangedEventArgs(BookState state, Guid updatedBy) : base(state)
        {
            NewSubTitle = state.SubTitle;
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookSeriesTitleChangedEventArgs : BookEventArgs
    {
        public string NewSeriesTitle { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookSeriesTitleChanged";

        public BookSeriesTitleChangedEventArgs(BookState state, Guid updatedBy) : base(state)
        {
            NewSeriesTitle = state.SeriesTitle;
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookSeriesVolumeChangedEventArgs : BookEventArgs
    {
        public string NewSeriesVolume { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookSeriesVolumeChanged";

        public BookSeriesVolumeChangedEventArgs(BookState state, Guid updatedBy) : base(state)
        {
            NewSeriesVolume = state.SeriesTitle;
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookGenreChangedEventArgs : BookEventArgs
    {
        public GenreModel NewGenre { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookGenreChanged";

        public BookGenreChangedEventArgs(BookState state, Guid updatedBy) : base(state)
        {
            NewGenre = state.Genre;
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookLanguageChangedEventArgs : BookEventArgs
    {
        public LanguageModel NewLanguage { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookLanguageChanged";

        public BookLanguageChangedEventArgs(BookState state, Guid updatedBy) : base(state)
        {
            NewLanguage = state.Language;
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookSynopsisUpdatedEventArgs : BookEventArgs
    {
        public string NewSynopsis { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookSynopsisUpdated";

        public BookSynopsisUpdatedEventArgs(BookState state, Guid updatedBy) : base(state)
        {
            NewSynopsis = state.SeriesTitle;
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookCompletedEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookCompleted";

        public BookCompletedEventArgs(BookState state, Guid updatedBy) : base(state)
        {
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookSetAsInProgressEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookSetAsInProgress";

        public BookSetAsInProgressEventArgs(BookState state, Guid updatedBy) : base(state)
        {
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookAbandonedEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookAbandoned";

        public BookAbandonedEventArgs(BookState state, Guid updatedBy) : base(state)
        {
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookRevivedEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookRevived";

        public BookRevivedEventArgs(BookState state, Guid updatedBy) : base(state)
        {
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookPublishedEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookPublished";

        public BookPublishedEventArgs(BookState state, Guid updatedBy) : base(state)
        {
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }
}