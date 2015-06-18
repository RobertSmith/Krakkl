using System;
using System.Collections.Generic;
using System.Linq;
using Krakkl.Authorship.Models;

namespace Krakkl.Authorship.Aggregates
{
    public class BookAggregate
    {
        private BookState _state;
        private int _version;
        private List<object> _uncommittedEvents = new List<object>();

        public Guid Key => _state.Key;

        public BookAggregate(BookState state)
        {
            _state = state;
        }

        public BookAggregate(IEnumerable<object> events)
        {
            foreach (var e in events)
                Apply(e);
        }

        public void StartANewBook(AuthorModel author, LanguageModel language)
        {
            if (_state == null)
                _state = new BookState();

            if (_state.Key != Guid.Empty)
                throw new Exception("Can not start a new book, this one is already in progress.");

            _state.Key = Guid.NewGuid();
            _state.Authors.Add(author);
            _state.Language = language;

            Publish(new BookCreatedEventArgs(_state, author.Key));
        }

        public void AddAuthor(Guid authorKey, AuthorModel newAuthor)
        {
            if (_state.Authors.Any(author => author.Key == newAuthor.Key))
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Authors.Add(newAuthor);

            Publish(new AuthorAddedToBookEventArgs(_state, newAuthor, authorKey));
        }

        public void RemoveAuthor(Guid authorKey, AuthorModel removedAuthor)
        {
            if (_state.Authors.All(a => a.Key != removedAuthor.Key))
                return;

            if (_state.Authors.Count == 1)
                throw new Exception("There must be at least one author assigned to a book");

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Authors.Remove(_state.Authors.Single(a => a.Key == removedAuthor.Key));

            Publish(new AuthorRemovedFromBookEventArgs(_state, removedAuthor, _state.Authors, authorKey));
        }

        public void Retitle(Guid authorKey, string newTitle)
        {
            if (_state.Title == newTitle)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Title = newTitle;

            Publish(new BookRetitledEventArgs(_state, authorKey));
        }

        public void ChangeSubTitle(Guid authorKey, string newSubTitle)
        {
            if (_state.SubTitle == newSubTitle)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.SubTitle = newSubTitle;

            Publish(new BookSubTitleChangedEventArgs(_state, authorKey));
        }

        public void ChangeSeriesTitle(Guid authorKey, string newSeriesTitle)
        {
            if (_state.SeriesTitle == newSeriesTitle)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.SeriesTitle = newSeriesTitle;

            Publish(new BookSeriesTitleChangedEventArgs(_state, authorKey));
        }

        public void ChangeSeriesVolume(Guid authorKey, string newSeriesVolume)
        {
            if (_state.SeriesVolume == newSeriesVolume)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.SeriesVolume = newSeriesVolume;

            Publish(new BookSeriesVolumeChangedEventArgs(_state, authorKey));
        }

        public void ChangeGenre(Guid authorKey, GenreModel newGenre)
        {
            if (_state.Genre == newGenre)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Genre = newGenre;

            Publish(new BookGenreChangedEventArgs(_state, authorKey));
        }

        public void ChangeEditorLanguage(Guid authorKey, LanguageModel newLanguage)
        {
            if (_state.Language == newLanguage)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Language = newLanguage;

            Publish(new BookLanguageChangedEventArgs(_state, authorKey));
        }

        public void UpdateSynopsis(Guid authorKey, string newSynopsis)
        {
            if (_state.Synopsis == newSynopsis)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Synopsis = newSynopsis;

            Publish(new BookSynopsisUpdatedEventArgs(_state, authorKey));
        }

        public void CompleteBook(Guid authorKey)
        {
            if (_state.Completed)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Completed = true;

            Publish(new BookCompletedEventArgs(_state, authorKey));
        }

        public void SetBookAsInProgress(Guid authorKey)
        {
            if (!_state.Completed)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Completed = false;

            Publish(new BookSetAsInProgressEventArgs(_state, authorKey));
        }

        public void AbandonBook(Guid authorKey)
        {
            if (_state.Abandoned)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Abandoned = true;

            Publish(new BookAbandonedEventArgs(_state, authorKey));
        }

        public void ReviveBook(Guid authorKey)
        {
            if (!_state.Abandoned)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Abandoned = false;

            Publish(new BookRevivedEventArgs(_state, authorKey));
        }

        public void PublishBook(Guid authorKey)
        {
            if (_state.Published)
                return;

            if (!AuthorCanEditBook(authorKey))
                throw new Exception("This author is not valid for updating this book");

            _state.Published = true;

            Publish(new BookPublishedEventArgs(_state, authorKey));
        }

        private bool AuthorCanEditBook(Guid authorKey)
        {
            return _state.Authors.Any(author => author.Key.Equals(authorKey));
        }

        private void Publish(object e)
        {
            _uncommittedEvents.Add(e);
        }

        #region Re-Hydrate Aggregate

        private void Apply(object e)
        {
            _version++;
            RedirectToWhen.InvokeEventOptional(this, e);
        }

        // ReSharper disable UnusedMember.Local
        private void When(BookCreatedEventArgs e)
        {
            _state.Key = e.BookKey;
            _state.Authors.Add(e.AddedAuthor);
            _state.Language = new LanguageModel(e.LanguageKey, e.LanguageName);
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