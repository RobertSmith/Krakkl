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
        public string EventType { get; private set; }

        public BookEventArgs(Guid bookKey, string eventType)
        {
            BookKey = bookKey;
            EventType = eventType;
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
        public LanguageModel Language { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid CreatedBy { get; private set; }

        public BookCreatedEventArgs(Guid bookKey, AuthorModel addedAuthor, LanguageModel language, Guid createdBy) : base(bookKey, "BookCreated")
        {
            AddedAuthor = addedAuthor;
            Language = language;
            CreatedAt = TimeStamp;
            CreatedBy = createdBy;
        }
    }

    internal sealed class AuthorAddedToBookEventArgs : BookEventArgs
    {
        public AuthorModel AddedAuthor { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }

        public AuthorAddedToBookEventArgs(Guid bookKey, AuthorModel addedAuthor, Guid updatedBy) : base(bookKey, "AuthorAddedToBook")
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

        public AuthorRemovedFromBookEventArgs(Guid bookKey, AuthorModel removedAuthor, List<AuthorModel> validAuthors, Guid updatedBy) : base(bookKey, "AuthorRemovedFromBook")
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

        public BookRetitledEventArgs(Guid bookKey, string title, Guid updatedBy) : base(bookKey, "BookRetitled")
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

        public BookSubTitleChangedEventArgs(Guid bookKey, string subTitle, Guid updatedBy) : base(bookKey, "BookSubTitleChanged")
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

        public BookSeriesTitleChangedEventArgs(Guid bookKey, string seriesTitle, Guid updatedBy) : base(bookKey, "BookSeriesTitleChanged")
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

        public BookSeriesVolumeChangedEventArgs(Guid bookKey, string seriesTitle, Guid updatedBy) : base(bookKey, "BookSeriesVolumeChanged")
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

        public BookGenreChangedEventArgs(Guid bookKey, GenreModel genre, Guid updatedBy) : base(bookKey, "BookGenreChanged")
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

        public BookLanguageChangedEventArgs(Guid bookKey, LanguageModel language, Guid updatedBy) : base(bookKey, "BookLanguageChanged")
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

        public BookSynopsisUpdatedEventArgs(Guid bookKey, string synopsys, Guid updatedBy) : base(bookKey, "BookSynopsisUpdated")
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

        public BookCompletedEventArgs(Guid bookKey, Guid updatedBy) : base(bookKey, "BookCompleted")
        {
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookSetAsInProgressEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }

        public BookSetAsInProgressEventArgs(Guid bookKey, Guid updatedBy) : base(bookKey, "BookSetAsInProgress")
        {
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookAbandonedEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }

        public BookAbandonedEventArgs(Guid bookKey, Guid updatedBy) : base(bookKey, "BookAbandoned")
        {
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookRevivedEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }

        public BookRevivedEventArgs(Guid bookKey, Guid updatedBy) : base(bookKey, "BookRevived")
        {
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookPublishedEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }

        public BookPublishedEventArgs(Guid bookKey, Guid updatedBy) : base(bookKey, "BookPublished")
        {
            UpdatedAt = TimeStamp;
            UpdatedBy = updatedBy;
        }
    }
}