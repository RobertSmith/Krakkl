using System;
using System.Collections.Generic;
using System.Linq;
using Krakkl.Authorship.ValueObjects;

namespace Krakkl.Authorship.Book.Aggregate
{
    public class BookAggregate
    {
        private Entities.Book _book;

        public List<object> UncommittedEvents = new List<object>();
        public Guid Key => _book.Key;
        public Entities.Book Book => _book;

        public BookAggregate()
        {
            _book = new Entities.Book();
        }

        public BookAggregate(Entities.Book state)
        {
            _book = state ?? new Entities.Book();
        }

        public BookAggregate(Entities.Book state, IEnumerable<object> events)
        {
            _book = state ?? new Entities.Book();

            foreach (var e in events)
                Apply(e);
        }

        internal void StartANewBook(ValueObjects.Author author, Language language)
        {
            if (_book == null)
                _book = new Entities.Book();

            if (_book.Key != Guid.Empty)
                throw new Exception("Can not start a new book, this one is already in progress.");

            var bookKey = Guid.NewGuid();

            Publish(new BookCreatedEventArgs(bookKey, author, language, DateTime.UtcNow, author.Key));
        }

        internal void AddAuthor(Guid authorKey, ValueObjects.Author newAuthor)
        {
            if (_book.Authors.Any(author => author.Key == newAuthor.Key))
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            Publish(new AuthorAddedToBookEventArgs(_book.Key, newAuthor, DateTime.UtcNow, authorKey));
        }

        internal void RemoveAuthor(Guid authorKey, ValueObjects.Author removedAuthor)
        {
            if (_book.Authors.All(a => a.Key != removedAuthor.Key))
                return;

            if (_book.Authors.Count == 1)
                throw new Exception("There must be at least one author assigned to a book");

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            Publish(new AuthorRemovedFromBookEventArgs(_book.Key, removedAuthor, _book.Authors, DateTime.UtcNow, authorKey));
        }

        internal void Retitle(Guid authorKey, string newTitle)
        {
            if (_book.Title == newTitle)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _book.Title = newTitle;

            Publish(new BookRetitledEventArgs(_book.Key, newTitle, DateTime.UtcNow, authorKey));
        }

        internal void ChangeSubTitle(Guid authorKey, string newSubTitle)
        {
            if (_book.SubTitle == newSubTitle)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            Publish(new BookSubTitleChangedEventArgs(_book.Key, newSubTitle, DateTime.UtcNow, authorKey));
        }

        internal void ChangeSeriesTitle(Guid authorKey, string newSeriesTitle)
        {
            if (_book.SeriesTitle == newSeriesTitle)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            Publish(new BookSeriesTitleChangedEventArgs(_book.Key, newSeriesTitle, DateTime.UtcNow, authorKey));
        }

        internal void ChangeSeriesVolume(Guid authorKey, string newSeriesVolume)
        {
            if (_book.SeriesVolume == newSeriesVolume)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            Publish(new BookSeriesVolumeChangedEventArgs(_book.Key, newSeriesVolume, DateTime.UtcNow, authorKey));
        }

        internal void ChangeGenre(Guid authorKey, Genre newGenre)
        {
            if (_book.Genre == newGenre)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            Publish(new BookGenreChangedEventArgs(_book.Key, newGenre, DateTime.UtcNow, authorKey));
        }

        internal void ChangeEditorLanguage(Guid authorKey, Language newLanguage)
        {
            if (_book.Language == newLanguage)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            Publish(new BookLanguageChangedEventArgs(_book.Key, newLanguage, DateTime.UtcNow, authorKey));
        }

        internal void UpdateSynopsis(Guid authorKey, string newSynopsis)
        {
            if (_book.Synopsis == newSynopsis)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _book.Synopsis = newSynopsis;

            Publish(new BookSynopsisUpdatedEventArgs(_book.Key, newSynopsis, DateTime.UtcNow, authorKey));
        }

        internal void CompleteBook(Guid authorKey)
        {
            if (_book.Completed)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            Publish(new BookCompletedEventArgs(_book.Key, DateTime.UtcNow, authorKey));
        }

        internal void SetBookAsInProgress(Guid authorKey)
        {
            if (!_book.Completed)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _book.Completed = false;

            Publish(new BookSetAsInProgressEventArgs(_book.Key, DateTime.UtcNow, authorKey));
        }

        internal void AbandonBook(Guid authorKey)
        {
            if (_book.Abandoned)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _book.Abandoned = true;

            Publish(new BookAbandonedEventArgs(_book.Key, DateTime.UtcNow, authorKey));
        }

        internal void ReviveBook(Guid authorKey)
        {
            if (!_book.Abandoned)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _book.Abandoned = false;

            Publish(new BookRevivedEventArgs(_book.Key, DateTime.UtcNow, authorKey));
        }

        internal void PublishBook(Guid authorKey)
        {
            if (_book.Published)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _book.Published = true;

            Publish(new BookPublishedEventArgs(_book.Key, DateTime.UtcNow, authorKey));
        }

        private bool AuthorCanEditBook(Guid authorKey)
        {
            return _book.Authors.Any(author => author.Key.Equals(authorKey));
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
            if (_book == null)
                _book = new Entities.Book();

            _book.Key = e.BookKey;
            _book.Authors.Add(e.AddedAuthor);
            _book.Language = e.Language;
        }

        private void When(AuthorAddedToBookEventArgs e)
        {
            _book.Authors.Add(e.AddedAuthor);
        }

        private void When(AuthorRemovedFromBookEventArgs e)
        {
            _book.Authors.Remove(_book.Authors.Single(a => a.Key == e.RemovedAuthor.Key));
        }

        private void When(BookRetitledEventArgs e)
        {
            _book.Title = e.NewTitle;
        }

        private void When(BookSubTitleChangedEventArgs e)
        {
            _book.SubTitle = e.NewSubTitle;
        }

        private void When(BookSeriesTitleChangedEventArgs e)
        {
            _book.SeriesTitle = e.NewSeriesTitle;
        }

        private void When(BookSeriesVolumeChangedEventArgs e)
        {
            _book.SeriesVolume = e.NewSeriesVolume;
        }

        private void When(BookGenreChangedEventArgs e)
        {
            _book.Genre = e.NewGenre;
        }

        private void When(BookLanguageChangedEventArgs e)
        {
            _book.Language = e.NewLanguage;
        }

        private void When(BookSynopsisUpdatedEventArgs e)
        {
            _book.Synopsis = e.NewSynopsis;
        }

        // ReSharper disable UnusedParameter.Local
        private void When(BookCompletedEventArgs e)
        {
            _book.Completed = true;
        }

        private void When(BookSetAsInProgressEventArgs e)
        {
            _book.Completed = false;
        }

        private void When(BookAbandonedEventArgs e)
        {
            _book.Abandoned = true;
        }

        private void When(BookRevivedEventArgs e)
        {
            _book.Abandoned = false;
        }

        private void When(BookPublishedEventArgs e)
        {
            _book.Published = true;
        }
        // ReSharper restore UnusedParameter.Local
        // ReSharper restore UnusedMember.Local

        #endregion
    }
}