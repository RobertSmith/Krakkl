using System;
using System.Collections.Generic;
using System.Linq;
using Krakkl.Authorship.Models;

namespace Krakkl.Authorship.Core
{
    // all event arguments should inherit from EventArgs
    internal class BookCreatedEventArgs : EventArgs
    {
        public Guid BookKey { get; private set; }
        public Guid AuthorKey { get; private set; }
        public string AuthorName { get; private set; }
        public string LanguageKey { get; private set; }
        public string LanguageName { get; private set; }
        public string Title { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid CreatedBy { get; private set; }
        public string EventType => "BookCreated";
        public string EventSource => "Book.Authorship.Krakkl";

        public BookCreatedEventArgs(BookState state)
        {
            BookKey = state.Key;
            AuthorKey = state.Authors.First().Key;
            AuthorName = state.Authors.First().Name;
            LanguageKey = state.Language?.Key;
            LanguageName = state.Language?.Name;
            Title = state.Title;
            CreatedAt = state.CreatedAt;
            CreatedBy = state.CreatedBy;
        }
    }

    internal class AuthorAddedToBookEventArgs : EventArgs
    {
        public Guid BookKey { get; private set; }
        public AuthorModel NewAuthor { get; private set; }
        public List<AuthorModel> ValidAuthors { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "AuthorAddedToBook";
        public string EventSource => "Book.Authorship.Krakkl";

        public AuthorAddedToBookEventArgs(BookState state, AuthorModel newAuthor)
        {
            BookKey = state.Key;
            NewAuthor = newAuthor;
            ValidAuthors = state.Authors;
            UpdatedAt = state.UpdatedAt;
            UpdatedBy = state.UpdatedBy;
        }
    }

    internal class AuthorRemovedFromBookEventArgs : EventArgs
    {
        public Guid BookKey { get; private set; }
        public AuthorModel RemovedAuthor { get; private set; }
        public List<AuthorModel> ValidAuthors { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "AuthorRemovedFromBook";
        public string EventSource => "Book.Authorship.Krakkl";

        public AuthorRemovedFromBookEventArgs(BookState state, AuthorModel removedAuthor)
        {
            BookKey = state.Key;
            RemovedAuthor = removedAuthor;
            ValidAuthors = state.Authors;
            UpdatedAt = state.UpdatedAt;
            UpdatedBy = state.UpdatedBy;
        }
    }

    internal class BookRetitledEventArgs : EventArgs
    {
        public Guid BookKey { get; private set; }
        public string OldTitle { get; private set; }
        public string NewTitle { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookRetitled";
        public string EventSource => "Book.Authorship.Krakkl";

        public BookRetitledEventArgs(BookState state, string oldTitle)
        {
            BookKey = state.Key;
            OldTitle = oldTitle;
            NewTitle = state.Title;
            UpdatedAt = state.UpdatedAt;
            UpdatedBy = state.UpdatedBy;
        }
    }

    internal class BookSubTitleChangedEventArgs : EventArgs
    {
        public Guid BookKey { get; private set; }
        public string OldSubTitle { get; private set; }
        public string NewSubTitle { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookSubTitleChanged";
        public string EventSource => "Book.Authorship.Krakkl";

        public BookSubTitleChangedEventArgs(BookState state, string oldSubTitle)
        {
            BookKey = state.Key;
            OldSubTitle = oldSubTitle;
            NewSubTitle = state.SubTitle;
            UpdatedAt = state.UpdatedAt;
            UpdatedBy = state.UpdatedBy;
        }
    }

    internal class BookSeriesTitleChangedEventArgs : EventArgs
    {
        public Guid BookKey { get; private set; }
        public string OldSeriesTitle { get; private set; }
        public string NewSeriesTitle { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookSeriesTitleChanged";
        public string EventSource => "Book.Authorship.Krakkl";

        public BookSeriesTitleChangedEventArgs(BookState state, string oldSeriesTitle)
        {
            BookKey = state.Key;
            OldSeriesTitle = oldSeriesTitle;
            NewSeriesTitle = state.SeriesTitle;
            UpdatedAt = state.UpdatedAt;
            UpdatedBy = state.UpdatedBy;
        }
    }

    internal class BookSeriesVolumeChangedEventArgs : EventArgs
    {
        public Guid BookKey { get; private set; }
        public string OldSeriesVolume { get; private set; }
        public string NewSeriesVolume { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookSeriesVolumeChanged";
        public string EventSource => "Book.Authorship.Krakkl";

        public BookSeriesVolumeChangedEventArgs(BookState state, string oldSeriesVolume)
        {
            BookKey = state.Key;
            OldSeriesVolume = oldSeriesVolume;
            NewSeriesVolume = state.SeriesTitle;
            UpdatedAt = state.UpdatedAt;
            UpdatedBy = state.UpdatedBy;
        }
    }

    internal class BookGenreChangedEventArgs : EventArgs
    {
        public Guid BookKey { get; private set; }
        public GenreModel OldGenre { get; private set; }
        public GenreModel NewGenre { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookGenreChanged";
        public string EventSource => "Book.Authorship.Krakkl";

        public BookGenreChangedEventArgs(BookState state, GenreModel oldGenre)
        {
            BookKey = state.Key;
            OldGenre = oldGenre;
            NewGenre = state.Genre;
            UpdatedAt = state.UpdatedAt;
            UpdatedBy = state.UpdatedBy;
        }
    }

    internal class BookLanguageChangedEventArgs : EventArgs
    {
        public Guid BookKey { get; private set; }
        public LanguageModel OldLanguage { get; private set; }
        public LanguageModel NewLanguage { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookLanguageChanged";
        public string EventSource => "Book.Authorship.Krakkl";

        public BookLanguageChangedEventArgs(BookState state, LanguageModel oldLanguage)
        {
            BookKey = state.Key;
            OldLanguage = oldLanguage;
            NewLanguage = state.Language;
            UpdatedAt = state.UpdatedAt;
            UpdatedBy = state.UpdatedBy;
        }
    }

    internal class BookSynopsisUpdatedEventArgs : EventArgs
    {
        public Guid BookKey { get; private set; }
        public string OldSynopsis { get; private set; }
        public string NewSynopsis { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "SynopsisUpdated";
        public string EventSource => "Book.Authorship.Krakkl";

        public BookSynopsisUpdatedEventArgs(BookState state, string oldSynopsis)
        {
            BookKey = state.Key;
            OldSynopsis = oldSynopsis;
            NewSynopsis = state.SeriesTitle;
            UpdatedAt = state.UpdatedAt;
            UpdatedBy = state.UpdatedBy;
        }
    }

    internal class BookCompletedEventArgs : EventArgs
    {
        public Guid BookKey { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookCompleted";
        public string EventSource => "Book.Authorship.Krakkl";

        public BookCompletedEventArgs(BookState state)
        {
            BookKey = state.Key;
            UpdatedAt = state.UpdatedAt;
            UpdatedBy = state.UpdatedBy;
        }
    }

    internal class BookSetAsInProgressEventArgs : EventArgs
    {
        public Guid BookKey { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookSetAsInProgress";
        public string EventSource => "Book.Authorship.Krakkl";

        public BookSetAsInProgressEventArgs(BookState state)
        {
            BookKey = state.Key;
            UpdatedAt = state.UpdatedAt;
            UpdatedBy = state.UpdatedBy;
        }
    }

    internal class BookAbandonedEventArgs : EventArgs
    {
        public Guid BookKey { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookAbandoned";
        public string EventSource => "Book.Authorship.Krakkl";

        public BookAbandonedEventArgs(BookState state)
        {
            BookKey = state.Key;
            UpdatedAt = state.UpdatedAt;
            UpdatedBy = state.UpdatedBy;
        }
    }

    internal class BookRevivedEventArgs : EventArgs
    {
        public Guid BookKey { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookRevived";
        public string EventSource => "Book.Authorship.Krakkl";

        public BookRevivedEventArgs(BookState state)
        {
            BookKey = state.Key;
            UpdatedAt = state.UpdatedAt;
            UpdatedBy = state.UpdatedBy;
        }
    }

    internal class BookPublishedEventArgs : EventArgs
    {
        public Guid BookKey { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid UpdatedBy { get; private set; }
        public string EventType => "BookPublisheds";
        public string EventSource => "Book.Authorship.Krakkl";

        public BookPublishedEventArgs(BookState state)
        {
            BookKey = state.Key;
            UpdatedAt = state.UpdatedAt;
            UpdatedBy = state.UpdatedBy;
        }
    }
}