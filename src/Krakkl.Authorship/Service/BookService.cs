using System;
using Krakkl.Authorship.Aggregates;
using Krakkl.Authorship.Cache;
using Krakkl.Authorship.Models;

namespace Krakkl.Authorship.Service
{
    /// <summary>
    /// This is the only public class available for the Authorship\Book domain. It is an Anti Corruption Layer and will provide translation 
    /// to and from the book aggregate.
    /// </summary>
    public class BookService
    {
        public long CacheCount => BookAggregateCache.Count();

        private readonly MessagingService _messagingService;

        public BookService()
        {
            //TODO: Init Moderation Module
            _messagingService = new MessagingService();
        }

        public Guid When(StartANewBookCommand cmd)
        {
            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            if (string.IsNullOrEmpty(cmd.AuthorName))
                throw new Exception("Author Name is required");

            if (string.IsNullOrEmpty(cmd.LanguageKey))
                throw new Exception("Language Key is required");

            if (string.IsNullOrEmpty(cmd.LanguageName))
                throw new Exception("Language Name is required");

            var author = new AuthorModel(cmd.AuthorKey, cmd.AuthorName);
            var language = new LanguageModel(cmd.LanguageKey, cmd.LanguageName);

            var bookAggregate = new BookAggregate(null);
            AddBookAggregateEventHandlers(bookAggregate);

            var bookKey = bookAggregate.StartANewBook(author, language);
            BookAggregateCache.UpdateItem(bookKey, bookAggregate);

            return bookKey;
        }

        public void When(AddAuthorToBookCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            if (cmd.NewAuthorKey == null)
                throw new Exception("New Author Key is required");

            if (string.IsNullOrEmpty(cmd.NewAuthorName))
                throw new Exception("New Author Name is required");

            var newAuthor = new AuthorModel(cmd.NewAuthorKey, cmd.NewAuthorName);
            var bookAggregate = FindBookAggregate(cmd.BookKey);

            bookAggregate.AddAuthor(cmd.AuthorKey, newAuthor);
            BookAggregateCache.UpdateItem(cmd.BookKey, bookAggregate);
        }

        public void When(RemoveAuthorFromBookCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            if (cmd.RemoveAuthorKey == null)
                throw new Exception("Remove Author Key is required");

            if (string.IsNullOrEmpty(cmd.RemoveAuthorName))
                throw new Exception("Remove Author Name is required");

            var removeAuthor = new AuthorModel(cmd.RemoveAuthorKey, cmd.RemoveAuthorName);
            var bookAggregate = FindBookAggregate(cmd.BookKey);

            bookAggregate.RemoveAuthor(cmd.AuthorKey, removeAuthor);
            BookAggregateCache.UpdateItem(cmd.BookKey, bookAggregate);
        }

        public void When(RetitleBookCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            if (string.IsNullOrEmpty(cmd.Title))
                cmd.Title = "Untitled";

            var bookAggregate = FindBookAggregate(cmd.BookKey);

            bookAggregate.Retitle(cmd.AuthorKey, cmd.Title);
            BookAggregateCache.UpdateItem(cmd.BookKey, bookAggregate);
        }

        public void When(ChangeBookSubtitleCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            var bookAggregate = FindBookAggregate(cmd.BookKey);

            bookAggregate.ChangeSubTitle(cmd.AuthorKey, cmd.SubTitle);
            BookAggregateCache.UpdateItem(cmd.BookKey, bookAggregate);
        }

        public void When(ChangeBookSeriesTitleCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            var bookAggregate = FindBookAggregate(cmd.BookKey);

            bookAggregate.ChangeSeriesTitle(cmd.AuthorKey, cmd.SeriesTitle);
            BookAggregateCache.UpdateItem(cmd.BookKey, bookAggregate);
        }

        public void When(ChangeBookSeriesVolumeCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            var bookAggregate = FindBookAggregate(cmd.BookKey);

            bookAggregate.ChangeSeriesVolume(cmd.AuthorKey, cmd.SeriesVolume);
            BookAggregateCache.UpdateItem(cmd.BookKey, bookAggregate);
        }

        public void When(ChangeBookGenreCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            if (string.IsNullOrEmpty(cmd.GenreKey))
                throw new Exception("Gnere Key is required");

            if (string.IsNullOrEmpty(cmd.GenreName))
                throw new Exception("Gnere Name is required");

            var newGenre = new GenreModel(cmd.GenreKey, cmd.GenreName, cmd.IsFiction);
            var bookAggregate = FindBookAggregate(cmd.BookKey);

            bookAggregate.ChangeGenre(cmd.AuthorKey, newGenre);
            BookAggregateCache.UpdateItem(cmd.BookKey, bookAggregate);
        }

        public void When(ChangeBookEditorLanguageCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            if (string.IsNullOrEmpty(cmd.LanguageKey))
                throw new Exception("Language Key is required");

            if (string.IsNullOrEmpty(cmd.LanguageName))
                throw new Exception("Language Name is required");

            var newLanguage = new LanguageModel(cmd.LanguageKey, cmd.LanguageName);
            var bookAggregate = FindBookAggregate(cmd.BookKey);

            bookAggregate.ChangeEditorLanguage(cmd.AuthorKey, newLanguage);
            BookAggregateCache.UpdateItem(cmd.BookKey, bookAggregate);
        }

        public void When(ChangeBookSynopsisCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            var bookAggregate = FindBookAggregate(cmd.BookKey);

            bookAggregate.UpdateSynopsis(cmd.AuthorKey, cmd.Synopsis);
            BookAggregateCache.UpdateItem(cmd.BookKey, bookAggregate);
        }

        public void When(CompleteBookCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            var bookAggregate = FindBookAggregate(cmd.BookKey);

            bookAggregate.CompleteBook(cmd.AuthorKey);
            BookAggregateCache.UpdateItem(cmd.BookKey, bookAggregate);
        }

        public void When(SetBookInProgressCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            var bookAggregate = FindBookAggregate(cmd.BookKey);

            bookAggregate.SetBookAsInProgress(cmd.AuthorKey);
            BookAggregateCache.UpdateItem(cmd.BookKey, bookAggregate);
        }

        public void When(AbandonBookCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            var bookAggregate = FindBookAggregate(cmd.BookKey);

            bookAggregate.AbandonBook(cmd.AuthorKey);
            BookAggregateCache.UpdateItem(cmd.BookKey, bookAggregate);
        }

        public void When(ReviveBookCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            var bookAggregate = FindBookAggregate(cmd.BookKey);

            bookAggregate.ReviveBook(cmd.AuthorKey);
            BookAggregateCache.UpdateItem(cmd.BookKey, bookAggregate);
        }

        public void When(PublishBookCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            var bookAggregate = FindBookAggregate(cmd.BookKey);

            bookAggregate.PublishBook(cmd.AuthorKey);
            BookAggregateCache.UpdateItem(cmd.BookKey, bookAggregate);
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
            bookAggregate.BookCreated += _messagingService.OnBookEventSendMessage;
            bookAggregate.AuthorAddedToBook += _messagingService.OnBookEventSendMessage;
            bookAggregate.AuthorRemovedFromBook += _messagingService.OnBookEventSendMessage;
            bookAggregate.BookRetitled += _messagingService.OnBookEventSendMessage;
            bookAggregate.BookSubTitleChanged += _messagingService.OnBookEventSendMessage;
            bookAggregate.BookSeriesTitleChanged += _messagingService.OnBookEventSendMessage;
            bookAggregate.BookSeriesVolumeChanged += _messagingService.OnBookEventSendMessage;
            bookAggregate.BookGenreChanged += _messagingService.OnBookEventSendMessage;
            bookAggregate.BookLanguageChanged += _messagingService.OnBookEventSendMessage;
            bookAggregate.BookSynopsisUpdated += _messagingService.OnBookEventSendMessage;
            bookAggregate.BookCompleted += _messagingService.OnBookEventSendMessage;
            bookAggregate.BookSetAsInProgress += _messagingService.OnBookEventSendMessage;
            bookAggregate.BookAbandoned += _messagingService.OnBookEventSendMessage;
            bookAggregate.BookRevived += _messagingService.OnBookEventSendMessage;
            bookAggregate.BookPublished += _messagingService.OnBookEventSendMessage;
        }

        #endregion
    }
}