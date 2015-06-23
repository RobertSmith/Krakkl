using System;
using System.Collections.Generic;
using System.Linq;
using Krakkl.Authorship.Book.Models;

namespace Krakkl.Authorship.Book.Aggregates
{
    public class BookAggregate
    {
        private BookState _state;

        public List<object> UncommittedEvents = new List<object>();
        public Guid Key => _state.Key;
        public BookState State => _state;

        public BookAggregate()
        {
            _state = new BookState();
        }

        public BookAggregate(BookState state)
        {
            _state = state ?? new BookState();
        }

        public BookAggregate(BookState state, IEnumerable<object> events)
        {
            _state = state ?? new BookState();

            foreach (var e in events)
                Apply(e);
        }

        internal void StartANewBook(AuthorModel author, LanguageModel language)
        {
            if (_state == null)
                _state = new BookState();

            if (_state.Key != Guid.Empty)
                throw new Exception("Can not start a new book, this one is already in progress.");

            var bookKey = Guid.NewGuid();

            Publish(new BookCreatedEventArgs(bookKey, author, language, DateTime.UtcNow, author.Key));
        }

        internal void AddAuthor(Guid authorKey, AuthorModel newAuthor)
        {
            if (_state.Authors.Any(author => author.Key == newAuthor.Key))
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            Publish(new AuthorAddedToBookEventArgs(_state.Key, newAuthor, DateTime.UtcNow, authorKey));
        }

        internal void RemoveAuthor(Guid authorKey, AuthorModel removedAuthor)
        {
            if (_state.Authors.All(a => a.Key != removedAuthor.Key))
                return;

            if (_state.Authors.Count == 1)
                throw new Exception("There must be at least one author assigned to a book");

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            Publish(new AuthorRemovedFromBookEventArgs(_state.Key, removedAuthor, _state.Authors, DateTime.UtcNow, authorKey));
        }

        internal void Retitle(Guid authorKey, string newTitle)
        {
            if (_state.Title == newTitle)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Title = newTitle;

            Publish(new BookRetitledEventArgs(_state.Key, newTitle, DateTime.UtcNow, authorKey));
        }

        internal void ChangeSubTitle(Guid authorKey, string newSubTitle)
        {
            if (_state.SubTitle == newSubTitle)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            Publish(new BookSubTitleChangedEventArgs(_state.Key, newSubTitle, DateTime.UtcNow, authorKey));
        }

        internal void ChangeSeriesTitle(Guid authorKey, string newSeriesTitle)
        {
            if (_state.SeriesTitle == newSeriesTitle)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            Publish(new BookSeriesTitleChangedEventArgs(_state.Key, newSeriesTitle, DateTime.UtcNow, authorKey));
        }

        internal void ChangeSeriesVolume(Guid authorKey, string newSeriesVolume)
        {
            if (_state.SeriesVolume == newSeriesVolume)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            Publish(new BookSeriesVolumeChangedEventArgs(_state.Key, newSeriesVolume, DateTime.UtcNow, authorKey));
        }

        internal void ChangeGenre(Guid authorKey, GenreModel newGenre)
        {
            if (_state.Genre == newGenre)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            Publish(new BookGenreChangedEventArgs(_state.Key, newGenre, DateTime.UtcNow, authorKey));
        }

        internal void ChangeEditorLanguage(Guid authorKey, LanguageModel newLanguage)
        {
            if (_state.Language == newLanguage)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            Publish(new BookLanguageChangedEventArgs(_state.Key, newLanguage, DateTime.UtcNow, authorKey));
        }

        internal void UpdateSynopsis(Guid authorKey, string newSynopsis)
        {
            if (_state.Synopsis == newSynopsis)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Synopsis = newSynopsis;

            Publish(new BookSynopsisUpdatedEventArgs(_state.Key, newSynopsis, DateTime.UtcNow, authorKey));
        }

        internal void CompleteBook(Guid authorKey)
        {
            if (_state.Completed)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            Publish(new BookCompletedEventArgs(_state.Key, DateTime.UtcNow, authorKey));
        }

        internal void SetBookAsInProgress(Guid authorKey)
        {
            if (!_state.Completed)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Completed = false;

            Publish(new BookSetAsInProgressEventArgs(_state.Key, DateTime.UtcNow, authorKey));
        }

        internal void AbandonBook(Guid authorKey)
        {
            if (_state.Abandoned)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Abandoned = true;

            Publish(new BookAbandonedEventArgs(_state.Key, DateTime.UtcNow, authorKey));
        }

        internal void ReviveBook(Guid authorKey)
        {
            if (!_state.Abandoned)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Abandoned = false;

            Publish(new BookRevivedEventArgs(_state.Key, DateTime.UtcNow, authorKey));
        }

        internal void PublishBook(Guid authorKey)
        {
            if (_state.Published)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Published = true;

            Publish(new BookPublishedEventArgs(_state.Key, DateTime.UtcNow, authorKey));
        }

        private bool AuthorCanEditBook(Guid authorKey)
        {
            return _state.Authors.Any(author => author.Key.Equals(authorKey));
        }

        private void Publish(object e)
        {
            UncommittedEvents.Add(e);
            Apply(e);
        }

        #region Re-Hydrate Aggregate

        private void Apply(object e)
        {
            RedirectToWhen.InvokeEventOptional(this, e);
        }

        // ReSharper disable UnusedMember.Local
        private void When(BookCreatedEventArgs e)
        {
            if (_state == null)
                _state = new BookState();

            _state.Key = e.BookKey;
            _state.Authors.Add(e.AddedAuthor);
            _state.Language = e.Language;
        }

        private void When(AuthorAddedToBookEventArgs e)
        {
            _state.Authors.Add(e.AddedAuthor);
        }

        private void When(AuthorRemovedFromBookEventArgs e)
        {
            _state.Authors.Remove(_state.Authors.Single(a => a.Key == e.RemovedAuthor.Key));
        }

        private void When(BookRetitledEventArgs e)
        {
            _state.Title = e.NewTitle;
        }

        private void When(BookSubTitleChangedEventArgs e)
        {
            _state.SubTitle = e.NewSubTitle;
        }

        private void When(BookSeriesTitleChangedEventArgs e)
        {
            _state.SeriesTitle = e.NewSeriesTitle;
        }

        private void When(BookSeriesVolumeChangedEventArgs e)
        {
            _state.SeriesVolume = e.NewSeriesVolume;
        }

        private void When(BookGenreChangedEventArgs e)
        {
            _state.Genre = e.NewGenre;
        }

        private void When(BookLanguageChangedEventArgs e)
        {
            _state.Language = e.NewLanguage;
        }

        private void When(BookSynopsisUpdatedEventArgs e)
        {
            _state.Synopsis = e.NewSynopsis;
        }

        // ReSharper disable UnusedParameter.Local
        private void When(BookCompletedEventArgs e)
        {
            _state.Completed = true;
        }

        private void When(BookSetAsInProgressEventArgs e)
        {
            _state.Completed = false;
        }

        private void When(BookAbandonedEventArgs e)
        {
            _state.Abandoned = true;
        }

        private void When(BookRevivedEventArgs e)
        {
            _state.Abandoned = false;
        }

        private void When(BookPublishedEventArgs e)
        {
            _state.Published = true;
        }
        // ReSharper restore UnusedParameter.Local
        // ReSharper restore UnusedMember.Local

        #endregion
    }
}