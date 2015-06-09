using System;
using Krakkl.Authorship.Cache;
using Krakkl.Authorship.Core;
using Krakkl.Authorship.Infrastructure;
using Krakkl.Authorship.Models;

namespace Krakkl.Authorship.Service
{
    /// <summary>
    /// This is the only public class available for the Authorship-Book domain. It is an Anti Corruption Layer and will provide translation 
    /// to and from the book aggregate.
    /// </summary>
    public class BookService
    {
        private readonly BookEventSourcePersistance _bookEventSourcePersistance;

        public BookService()
        {
            //TODO: Init Moderation Module
            _bookEventSourcePersistance = new BookEventSourcePersistance();
        }

        public Guid StartANewBook(Guid authorKey, string authorName, string languageKey, string languageName)
        {
            if (authorKey == null)
                throw new Exception("Author Key is required");

            if (string.IsNullOrEmpty(authorName))
                throw new Exception("Author Name is required");

            if (string.IsNullOrEmpty(languageKey))
                throw new Exception("Language Key is required");

            if (string.IsNullOrEmpty(languageName))
                throw new Exception("Language Name is required");

            var author = new AuthorModel(authorKey, authorName);
            var language = new LanguageModel(languageKey, languageName);

            var bookAggregate = new BookAggregate(null);
            AddBookAggregateEventHandlers(bookAggregate);

            var bookKey = bookAggregate.StartANewBook(author, language);
            BookAggregateCache.UpdateItem(bookKey, bookAggregate);

            return bookKey;
        }

        public void AddAnotherAuthor(Guid bookKey, Guid authorKey, Guid newAuthorKey, string newAuthorName)
        {
            if (bookKey == null)
                throw new Exception("Book Key is required");

            if (authorKey == null)
                throw new Exception("Author Key is required");

            if (newAuthorKey == null)
                throw new Exception("New Author Key is required");

            if (string.IsNullOrEmpty(newAuthorName))
                throw new Exception("New Author Name is required");

            var newAuthor = new AuthorModel(newAuthorKey, newAuthorName);
            var bookAggregate = FindBookAggregate(bookKey);

            bookAggregate.AddAuthor(authorKey, newAuthor);
            BookAggregateCache.UpdateItem(bookKey, bookAggregate);
        }

        public void RemoveAnotherAuthor(Guid bookKey, Guid authorKey, Guid removeAuthorKey, string removeAuthorName)
        {
            if (bookKey == null)
                throw new Exception("Book Key is required");

            if (authorKey == null)
                throw new Exception("Author Key is required");

            if (removeAuthorKey == null)
                throw new Exception("Remove Author Key is required");

            if (string.IsNullOrEmpty(removeAuthorName))
                throw new Exception("Remove Author Name is required");

            var removeAuthor = new AuthorModel(removeAuthorKey, removeAuthorName);
            var bookAggregate = FindBookAggregate(bookKey);

            bookAggregate.RemoveAuthor(authorKey, removeAuthor);
            BookAggregateCache.UpdateItem(bookKey, bookAggregate);
        }

        public void Retitle(Guid bookKey, Guid authorKey, string newTitle)
        {
            if (bookKey == null)
                throw new Exception("Book Key is required");

            if (authorKey == null)
                throw new Exception("Author Key is required");

            if (string.IsNullOrEmpty(newTitle))
                newTitle = "Untitled";

            var bookAggregate = FindBookAggregate(bookKey);

            bookAggregate.Retitle(authorKey, newTitle);
            BookAggregateCache.UpdateItem(bookKey, bookAggregate);
        }

        public void ChangeSubtitle(Guid bookKey, Guid authorKey, string subtitle)
        {
            if (bookKey == null)
                throw new Exception("Book Key is required");

            if (authorKey == null)
                throw new Exception("Author Key is required");

            var bookAggregate = FindBookAggregate(bookKey);

            bookAggregate.ChangeSubTitle(authorKey, subtitle);
            BookAggregateCache.UpdateItem(bookKey, bookAggregate);
        }

        public void ChangeSeriesTitle(Guid bookKey, Guid authorKey, string seriesTitle)
        {
            if (bookKey == null)
                throw new Exception("Book Key is required");

            if (authorKey == null)
                throw new Exception("Author Key is required");

            var bookAggregate = FindBookAggregate(bookKey);

            bookAggregate.ChangeSeriesTitle(authorKey, seriesTitle);
            BookAggregateCache.UpdateItem(bookKey, bookAggregate);
        }

        public void ChangeSeriesVolume(Guid bookKey, Guid authorKey, string seriesVolume)
        {
            if (bookKey == null)
                throw new Exception("Book Key is required");

            if (authorKey == null)
                throw new Exception("Author Key is required");

            var bookAggregate = FindBookAggregate(bookKey);

            bookAggregate.ChangeSeriesVolume(authorKey, seriesVolume);
            BookAggregateCache.UpdateItem(bookKey, bookAggregate);
        }

        public void ChangeGenre(Guid bookKey, Guid authorKey, string genreKey, string genreName, bool isFiction)
        {
            if (bookKey == null)
                throw new Exception("Book Key is required");

            if (authorKey == null)
                throw new Exception("Author Key is required");

            if (string.IsNullOrEmpty(genreKey))
                throw new Exception("Gnere Key is required");

            if (string.IsNullOrEmpty(genreName))
                throw new Exception("Gnere Name is required");

            var newGenre = new GenreModel(genreKey, genreName, isFiction);
            var bookAggregate = FindBookAggregate(bookKey);

            bookAggregate.ChangeGenre(authorKey, newGenre);
            BookAggregateCache.UpdateItem(bookKey, bookAggregate);
        }

        public void ChangeLanguage(Guid bookKey, Guid authorKey, string languageKey, string languageName)
        {
            if (bookKey == null)
                throw new Exception("Book Key is required");

            if (authorKey == null)
                throw new Exception("Author Key is required");

            if (string.IsNullOrEmpty(languageKey))
                throw new Exception("Language Key is required");

            if (string.IsNullOrEmpty(languageName))
                throw new Exception("Language Name is required");

            var newLanguage = new LanguageModel(languageKey, languageName);
            var bookAggregate = FindBookAggregate(bookKey);

            bookAggregate.ChangeEditorLanguage(authorKey, newLanguage);
            BookAggregateCache.UpdateItem(bookKey, bookAggregate);
        }

        public void ChangeSynopsis(Guid bookKey, Guid authorKey, string newSynopsis)
        {
            if (bookKey == null)
                throw new Exception("Book Key is required");

            if (authorKey == null)
                throw new Exception("Author Key is required");

            var bookAggregate = FindBookAggregate(bookKey);

            bookAggregate.UpdateSynopsis(authorKey, newSynopsis);
            BookAggregateCache.UpdateItem(bookKey, bookAggregate);
        }

        public void CompleteBook(Guid bookKey, Guid authorKey)
        {
            if (bookKey == null)
                throw new Exception("Book Key is required");

            if (authorKey == null)
                throw new Exception("Author Key is required");

            var bookAggregate = FindBookAggregate(bookKey);

            bookAggregate.CompleteBook(authorKey);
            BookAggregateCache.UpdateItem(bookKey, bookAggregate);
        }

        public void SetBookInProgress(Guid bookKey, Guid authorKey)
        {
            if (bookKey == null)
                throw new Exception("Book Key is required");

            if (authorKey == null)
                throw new Exception("Author Key is required");

            var bookAggregate = FindBookAggregate(bookKey);

            bookAggregate.SetBookAsInProgress(authorKey);
            BookAggregateCache.UpdateItem(bookKey, bookAggregate);
        }

        public void AbandonBook(Guid bookKey, Guid authorKey)
        {
            if (bookKey == null)
                throw new Exception("Book Key is required");

            if (authorKey == null)
                throw new Exception("Author Key is required");

            var bookAggregate = FindBookAggregate(bookKey);

            bookAggregate.AbandonBook(authorKey);
            BookAggregateCache.UpdateItem(bookKey, bookAggregate);
        }

        public void ReviveBook(Guid bookKey, Guid authorKey)
        {
            if (bookKey == null)
                throw new Exception("Book Key is required");

            if (authorKey == null)
                throw new Exception("Author Key is required");

            var bookAggregate = FindBookAggregate(bookKey);

            bookAggregate.ReviveBook(authorKey);
            BookAggregateCache.UpdateItem(bookKey, bookAggregate);
        }

        public void PublishBook(Guid bookKey, Guid authorKey)
        {
            if (bookKey == null)
                throw new Exception("Book Key is required");

            if (authorKey == null)
                throw new Exception("Author Key is required");

            var bookAggregate = FindBookAggregate(bookKey);

            bookAggregate.PublishBook(authorKey);
            BookAggregateCache.UpdateItem(bookKey, bookAggregate);
        }

        #region Private Methods

        private BookAggregate FindBookAggregate(Guid key)
        {
            var bookAggregate = BookAggregateCache.Get(key) as BookAggregate;

            if (bookAggregate == null)
            {
                bookAggregate = new BookAggregate(key);
                AddBookAggregateEventHandlers(bookAggregate);
            }

            return bookAggregate;
        }

        private void AddBookAggregateEventHandlers(BookAggregate bookAggregate)
        {
            //TODO: Add more!
            bookAggregate.BookCreated += _bookEventSourcePersistance.OnBookCreated;
        }

        #endregion
    }
}
