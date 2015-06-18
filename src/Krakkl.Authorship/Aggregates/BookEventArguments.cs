using System;
using System.Collections.Generic;
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

        public BookEventArgs(Guid key)
        {
            BookKey = key;
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

        public BookCreatedEventArgs(Guid key, AuthorModel author, LanguageModel language, Guid createdBy) : base(key)
        {
            AddedAuthor = author;
            LanguageKey = language?.Key;
            LanguageName = language?.Name;
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

        public AuthorAddedToBookEventArgs(Guid key, AuthorModel addedAuthor, Guid updatedBy) : base(key)
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

        public AuthorRemovedFromBookEventArgs(Guid key, AuthorModel removedAuthor, List<AuthorModel> validAuthors, Guid updatedBy) : base(key)
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

        public BookRetitledEventArgs(Guid key, string title, Guid updatedBy) : base(key)
        {
            NewTitle = title;
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

        public BookSubTitleChangedEventArgs(Guid key, string subTitle, Guid updatedBy) : base(key)
        {
            NewSubTitle = subTitle;
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

        public BookSeriesTitleChangedEventArgs(Guid key, string seriesTitle, Guid updatedBy) : base(key)
        {
            NewSeriesTitle = seriesTitle;
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

        public BookSeriesVolumeChangedEventArgs(Guid key, string seriesTitle, Guid updatedBy) : base(key)
        {
            NewSeriesVolume = seriesTitle;
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

        public BookGenreChangedEventArgs(Guid key, GenreModel genre, Guid updatedBy) : base(key)
        {
            NewGenre = genre;
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

        public BookLanguageChangedEventArgs(Guid key, LanguageModel language, Guid updatedBy) : base(key)
        {
            NewLanguage = language;
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

        public BookSynopsisUpdatedEventArgs(Guid key, string synopsys, Guid updatedBy) : base(key)
        {
            NewSynopsis = synopsys;
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookCompletedEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookCompleted";

        public BookCompletedEventArgs(Guid key, Guid updatedBy) : base(key)
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

        public BookSetAsInProgressEventArgs(Guid key, Guid updatedBy) : base(key)
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

        public BookAbandonedEventArgs(Guid key, Guid updatedBy) : base(key)
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

        public BookRevivedEventArgs(Guid key, Guid updatedBy) : base(key)
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

        public BookPublishedEventArgs(Guid key, Guid updatedBy) : base(key)
        {
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }
}