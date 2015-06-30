using System;
using System.Collections.Generic;
using Krakkl.Authorship.ValueObjects;
using Newtonsoft.Json;

namespace Krakkl.Authorship.Book.Aggregate
{
    // all event arguments should inherit from EventArgs
    internal class BookEventArgs : EventArgs
    {
        public Guid BookKey { get; private set; }
        public string EventSource => "Book.Authorship.Krakkl";
        public string EventType { get; private set; }

        public BookEventArgs(Guid bookKey, string eventType)
        {
            BookKey = bookKey;
            EventType = eventType;
        }

        public override string ToString()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, settings);
        }
    }

    internal sealed class BookCreatedEventArgs : BookEventArgs
    {
        public Author AddedAuthor { get; private set; }
        public Language Language { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid CreatedBy { get; private set; }

        public BookCreatedEventArgs(Guid bookKey, Author addedAuthor, Language language, DateTime createdAt, Guid createdBy) : base(bookKey, "BookCreated")
        {
            AddedAuthor = addedAuthor;
            Language = language;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
        }
    }

    internal sealed class AuthorAddedToBookEventArgs : BookEventArgs
    {
        public Author AddedAuthor { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }

        public AuthorAddedToBookEventArgs(Guid bookKey, Author addedAuthor, DateTime updatedAt, Guid updatedBy) : base(bookKey, "AuthorAddedToBook")
        {
            AddedAuthor = addedAuthor;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class AuthorRemovedFromBookEventArgs : BookEventArgs
    {
        public Author RemovedAuthor { get; private set; }
        public List<Author> ValidAuthors { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }

        public AuthorRemovedFromBookEventArgs(Guid bookKey, Author removedAuthor, List<Author> validAuthors, DateTime updatedAt, Guid updatedBy) : base(bookKey, "AuthorRemovedFromBook")
        {
            RemovedAuthor = removedAuthor;
            ValidAuthors = validAuthors;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookRetitledEventArgs : BookEventArgs
    {
        public string NewTitle { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }

        public BookRetitledEventArgs(Guid bookKey, string newTitle, DateTime updatedAt, Guid updatedBy) : base(bookKey, "BookRetitled")
        {
            NewTitle = newTitle;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookSubTitleChangedEventArgs : BookEventArgs
    {
        public string NewSubTitle { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }

        public BookSubTitleChangedEventArgs(Guid bookKey, string newSubTitle, DateTime updatedAt, Guid updatedBy) : base(bookKey, "BookSubTitleChanged")
        {
            NewSubTitle = newSubTitle;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookSeriesTitleChangedEventArgs : BookEventArgs
    {
        public string NewSeriesTitle { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }

        public BookSeriesTitleChangedEventArgs(Guid bookKey, string newSeriesTitle, DateTime updatedAt, Guid updatedBy) : base(bookKey, "BookSeriesTitleChanged")
        {
            NewSeriesTitle = newSeriesTitle;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookSeriesVolumeChangedEventArgs : BookEventArgs
    {
        public string NewSeriesVolume { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }

        public BookSeriesVolumeChangedEventArgs(Guid bookKey, string newSeriesVolume, DateTime updatedAt, Guid updatedBy) : base(bookKey, "BookSeriesVolumeChanged")
        {
            NewSeriesVolume = newSeriesVolume;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookGenreChangedEventArgs : BookEventArgs
    {
        public Genre NewGenre { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }

        public BookGenreChangedEventArgs(Guid bookKey, Genre newGenre, DateTime updatedAt, Guid updatedBy) : base(bookKey, "BookGenreChanged")
        {
            NewGenre = newGenre;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookLanguageChangedEventArgs : BookEventArgs
    {
        public Language NewLanguage { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }

        public BookLanguageChangedEventArgs(Guid bookKey, Language newLanguage, DateTime updatedAt, Guid updatedBy) : base(bookKey, "BookLanguageChanged")
        {
            NewLanguage = newLanguage;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookSynopsisUpdatedEventArgs : BookEventArgs
    {
        public string NewSynopsis { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }

        public BookSynopsisUpdatedEventArgs(Guid bookKey, string newSynopsis, DateTime updatedAt, Guid updatedBy) : base(bookKey, "BookSynopsisUpdated")
        {
            NewSynopsis = newSynopsis;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookCompletedEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }

        public BookCompletedEventArgs(Guid bookKey, DateTime updatedAt, Guid updatedBy) : base(bookKey, "BookCompleted")
        {
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookAbandonedEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }

        public BookAbandonedEventArgs(Guid bookKey, DateTime updatedAt, Guid updatedBy) : base(bookKey, "BookAbandoned")
        {
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookRevivedEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }

        public BookRevivedEventArgs(Guid bookKey, DateTime updatedAt, Guid updatedBy) : base(bookKey, "BookRevived")
        {
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class BookPublishedEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }

        public BookPublishedEventArgs(Guid bookKey, DateTime updatedAt, Guid updatedBy) : base(bookKey, "BookPublished")
        {
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }

    internal sealed class SetNewCoverArtEventArgs : BookEventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public Guid CoverArtKey { get; private set; }

        public SetNewCoverArtEventArgs(Guid bookKey, DateTime updatedAt, Guid updatedBy, Guid coverArtKey) : base(bookKey, "BookCoverArtSet")
        {
            CoverArtKey = coverArtKey;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }
}