using System;
using Krakkl.Authorship.Aggregates;
using Krakkl.Authorship.Models;
using Krakkl.Authorship.Repository;

namespace Krakkl.Authorship.Service
{
    /// <summary>
    /// This is the only class available for the Authorship\Book domain. It is an Anti Corruption Layer and will provide translation 
    /// to and from the book aggregate.
    /// </summary>
    public class BookService
    {
        private readonly MessagingService _messagingService;
        private readonly IBookAggregateRepository _bookAggregateRepository;

        public BookService(IBookAggregateRepository repository)
        {
            //TODO: Init Moderation Module
            _messagingService = new MessagingService();
            _bookAggregateRepository = repository;
        }

        public Guid Start(StartANewBookCommand cmd)
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

            var bookAggregate = new BookAggregate(new BookState());

            bookAggregate.StartANewBook(author, language);
            _bookAggregateRepository.Save(bookAggregate);

            return bookAggregate.Key;
        }

        public void Apply(object cmd)
        {
            RedirectToWhen.InvokeCommand(this, cmd);
        }

        // ReSharper disable UnusedMember.Local
        private void When(AddAuthorToBookCommand cmd)
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

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.AddAuthor(cmd.AuthorKey, newAuthor));
        }

        private void When(RemoveAuthorFromBookCommand cmd)
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

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.RemoveAuthor(cmd.AuthorKey, removeAuthor));
        }

        private void When(RetitleBookCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            if (string.IsNullOrEmpty(cmd.Title))
                cmd.Title = "Untitled";

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.Retitle(cmd.AuthorKey, cmd.Title));
        }

        private void When(ChangeBookSubtitleCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.ChangeSubTitle(cmd.AuthorKey, cmd.SubTitle));
        }

        private void When(ChangeBookSeriesTitleCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.ChangeSeriesTitle(cmd.AuthorKey, cmd.SeriesTitle));
        }

        private void When(ChangeBookSeriesVolumeCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.ChangeSeriesVolume(cmd.AuthorKey, cmd.SeriesVolume));
        }

        private void When(ChangeBookGenreCommand cmd)
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

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.ChangeGenre(cmd.AuthorKey, newGenre));
        }

        private void When(ChangeBookEditorLanguageCommand cmd)
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

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.ChangeEditorLanguage(cmd.AuthorKey, newLanguage));
        }

        private void When(ChangeBookSynopsisCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.UpdateSynopsis(cmd.AuthorKey, cmd.Synopsis));
        }

        private void When(CompleteBookCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.CompleteBook(cmd.AuthorKey));
        }

        private void When(SetBookInProgressCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.SetBookAsInProgress(cmd.AuthorKey));
        }

        private void When(AbandonBookCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.AbandonBook(cmd.AuthorKey));
        }

        private void When(ReviveBookCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.ReviveBook(cmd.AuthorKey));
        }

        private void When(PublishBookCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.PublishBook(cmd.AuthorKey));
        }
        // ReSharper restore UnusedMember.Local

        private void Act<T>(Guid key, Action<BookAggregate> action)
        {
            var aggregate = _bookAggregateRepository.FindByKey<T>(key);
            action(aggregate);
            _bookAggregateRepository.Save(aggregate);
        }
    }
}