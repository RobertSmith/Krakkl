﻿using System;
using System.Linq;
using Krakkl.Authorship.Models;
using Krakkl.Authorship.Repository;

namespace Krakkl.Authorship.Core
{
    internal class BookAggregate
    {
        private readonly BookState _state;

        public event EventHandler<BookCreatedEventArgs> BookCreated;
        public event EventHandler<AuthorAddedToBookEventArgs> AuthorAddedToBook;
        public event EventHandler<AuthorRemovedFromBookEventArgs> AuthorRemovedFromBook;
        public event EventHandler<BookRetitledEventArgs> BookRetitled;
        public event EventHandler<BookSubTitleChangedEventArgs> BookSubTitleChanged;
        public event EventHandler<BookSeriesTitleChangedEventArgs> BookSeriesTitleChanged;
        public event EventHandler<BookSeriesVolumeChangedEventArgs> BookSeriesVolumeChanged;
        public event EventHandler<BookGenreChangedEventArgs> BookGenreChanged;
        public event EventHandler<BookLanguageChangedEventArgs> BookLanguageChanged;
        public event EventHandler<SynopsisUpdatedEventArgs> SynopsisUpdated;
        public event EventHandler<BookCompletedEventArgs> BookCompleted;
        public event EventHandler<BookSetAsInProgressEventArgs> BookSetAsInProgress;
        public event EventHandler<BookAbandonedEventArgs> BookAbandoned;
        public event EventHandler<BookRevivedEventArgs> BookRevived;
        public event EventHandler<BookPublishedEventArgs> BookPublished;

        public Guid Key => _state.Key;

        public BookAggregate(BookState state)
        {
            _state = state;
        }

        public BookAggregate(Guid key)
        {
            var repo = new BookAggregateRepositoryOrchestrate();
            _state = repo.FindByKey(key);
        }

        public Guid StartANewBook(AuthorModel author, LanguageModel language)
        {
            if (_state.Key != Guid.Empty)
                throw new Exception("Can not start a new book, this one is already in progress.");

            _state.Key = Guid.NewGuid();
            _state.Authors.Add(author);
            _state.Language = language;
            _state.Title = "Untitled";
            _state.CreatedAt = DateTime.UtcNow;
            _state.CreatedBy = author.Key;

            // Fire BookCreated Event
            var target = BookCreated;
            target?.Invoke(this, new BookCreatedEventArgs(_state));

            return _state.Key;
        }

        public void AddAuthor(Guid authorKey, AuthorModel newAuthor)
        {
            if (_state.Authors.Contains(newAuthor))
                return;

            if (!IsValidAuthor(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Authors.Add(newAuthor);
            _state.UpdatedAt = DateTime.UtcNow;
            _state.UpdatedBy = authorKey;

            // Fire AuthorAddedToBook Event
            var target = AuthorAddedToBook;
            target?.Invoke(this, new AuthorAddedToBookEventArgs(_state, newAuthor));
        }

        public void RemoveAuthor(Guid authorKey, AuthorModel removedAuthor)
        {
            if (!_state.Authors.Contains(removedAuthor))
                return;

            if (_state.Authors.Count == 1)
                throw new Exception("There must be at least one author assigned to a book");

            if (!IsValidAuthor(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Authors.Remove(removedAuthor);
            _state.UpdatedAt = DateTime.UtcNow;
            _state.UpdatedBy = authorKey;

            // Fire AuthorRemovedFromBook Event
            var target = AuthorRemovedFromBook;
            target?.Invoke(this, new AuthorRemovedFromBookEventArgs(_state, removedAuthor));
        }

        public void Retitle(Guid authorKey, string newTitle)
        {
            if (_state.Title.Equals(newTitle))
                return;

            if (!IsValidAuthor(authorKey))
                throw new Exception("This author is not valid for updating this book");

            var oldTitle = _state.Title;

            _state.Title = newTitle;
            _state.UpdatedAt = DateTime.UtcNow;
            _state.UpdatedBy = authorKey;

            // Fire BookRetitled Event
            var target = BookRetitled;
            target?.Invoke(this, new BookRetitledEventArgs(_state, oldTitle));
        }

        public void ChangeSubTitle(Guid authorKey, string newSubTitle)
        {
            if (_state.SubTitle.Equals(newSubTitle))
                return;

            if (!IsValidAuthor(authorKey))
                throw new Exception("This author is not valid for updating this book");

            var oldSubTitle = _state.SubTitle;

            _state.SubTitle = newSubTitle;
            _state.UpdatedAt = DateTime.UtcNow;
            _state.UpdatedBy = authorKey;

            // Fire BookSubTitleChanged Event
            var target = BookSubTitleChanged;
            target?.Invoke(this, new BookSubTitleChangedEventArgs(_state, oldSubTitle));
        }

        public void ChangeSeriesTitle(Guid authorKey, string newSeriesTitle)
        {
            if (_state.SeriesTitle.Equals(newSeriesTitle))
                return;

            if (!IsValidAuthor(authorKey))
                throw new Exception("This author is not valid for updating this book");

            var oldSeriesTitle = _state.SeriesTitle;

            _state.SeriesTitle = newSeriesTitle;
            _state.UpdatedAt = DateTime.UtcNow;
            _state.UpdatedBy = authorKey;

            // Fire BookSeriesTitleChanged Event
            var target = BookSeriesTitleChanged;
            target?.Invoke(this, new BookSeriesTitleChangedEventArgs(_state, oldSeriesTitle));
        }

        public void ChangeSeriesVolume(Guid authorKey, string newSeriesVolume)
        {
            if (_state.SeriesVolume.Equals(newSeriesVolume))
                return;

            if (!IsValidAuthor(authorKey))
                throw new Exception("This author is not valid for updating this book");

            var oldSeriesVolume = _state.SeriesVolume;

            _state.SeriesVolume = newSeriesVolume;
            _state.UpdatedAt = DateTime.UtcNow;
            _state.UpdatedBy = authorKey;

            // Fire BookSeriesVolumeChanged Event
            var target = BookSeriesVolumeChanged;
            target?.Invoke(this, new BookSeriesVolumeChangedEventArgs(_state, oldSeriesVolume));
        }

        public void ChangeGenre(Guid authorKey, GenreModel newGenre)
        {
            if (_state.Genre == newGenre)
                return;

            if (!IsValidAuthor(authorKey))
                throw new Exception("This author is not valid for updating this book");

            var oldGenre = _state.Genre;

            _state.Genre = newGenre;
            _state.UpdatedAt = DateTime.UtcNow;
            _state.UpdatedBy = authorKey;

            // Fire BookGenreChanged Event
            var target = BookGenreChanged;
            target?.Invoke(this, new BookGenreChangedEventArgs(_state, oldGenre));
        }

        public void ChangeEditorLanguage(Guid authorKey, LanguageModel newLanguage)
        {
            if (_state.Language == newLanguage)
                return;

            if (!IsValidAuthor(authorKey))
                throw new Exception("This author is not valid for updating this book");

            var oldLanguage = _state.Language;

            _state.Language = newLanguage;
            _state.UpdatedAt = DateTime.UtcNow;
            _state.UpdatedBy = authorKey;

            // Fire BookLanguageChanged Event
            var target = BookLanguageChanged;
            target?.Invoke(this, new BookLanguageChangedEventArgs(_state, oldLanguage));
        }

        public void UpdateSynopsis(Guid authorKey, string newSynopsis)
        {
            if (_state.Synopsis.Equals(newSynopsis))
                return;

            if (!IsValidAuthor(authorKey))
                throw new Exception("This author is not valid for updating this book");

            var oldSynopsis = _state.Synopsis;

            _state.Synopsis = newSynopsis;
            _state.UpdatedAt = DateTime.UtcNow;
            _state.UpdatedBy = authorKey;

            // Fire SynopsisUpdated Event
            var target = SynopsisUpdated;
            target?.Invoke(this, new SynopsisUpdatedEventArgs(_state, oldSynopsis));
        }

        public void CompleteBook(Guid authorKey)
        {
            if (_state.Completed)
                return;

            if (!IsValidAuthor(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Completed = true;
            _state.UpdatedAt = DateTime.UtcNow;
            _state.UpdatedBy = authorKey;

            // Fire BookCompleted Event
            var target = BookCompleted;
            target?.Invoke(this, new BookCompletedEventArgs(_state));
        }

        public void SetBookAsInProgress(Guid authorKey)
        {
            if (!_state.Completed)
                return;

            if (!IsValidAuthor(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Completed = false;
            _state.UpdatedAt = DateTime.UtcNow;
            _state.UpdatedBy = authorKey;

            // Fire BookSetAsInProgress Event
            var target = BookSetAsInProgress;
            target?.Invoke(this, new BookSetAsInProgressEventArgs(_state));
        }

        public void AbandonBook(Guid authorKey)
        {
            if (_state.Abandoned)
                return;

            if (!IsValidAuthor(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Abandoned = true;
            _state.UpdatedAt = DateTime.UtcNow;
            _state.UpdatedBy = authorKey;

            // Fire BookAbandoned Event
            var target = BookAbandoned;
            target?.Invoke(this, new BookAbandonedEventArgs(_state));
        }

        public void ReviveBook(Guid authorKey)
        {
            if (!_state.Abandoned)
                return;

            if (!IsValidAuthor(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Abandoned = false;
            _state.UpdatedAt = DateTime.UtcNow;
            _state.UpdatedBy = authorKey;

            // Fire BookRevived Event
            var target = BookRevived;
            target?.Invoke(this, new BookRevivedEventArgs(_state));
        }

        public void PublishBook(Guid authorKey)
        {
            if (_state.Published)
                return;

            if (!IsValidAuthor(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Published = true;
            _state.UpdatedAt = DateTime.UtcNow;
            _state.UpdatedBy = authorKey;

            // Fire BookPublished Event
            var target = BookPublished;
            target?.Invoke(this, new BookPublishedEventArgs(_state));
        }

        private bool IsValidAuthor(Guid authorKey)
        {
            return _state.Authors.Any(author => author.Key.Equals(authorKey));
        }
    }
}
