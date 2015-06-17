﻿using System;
using Krakkl.Authorship.Aggregates;
using Krakkl.Authorship.Cache;
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
        public long CacheCount => BookAggregateCache.Count();

        private readonly MessagingService _messagingService;
        private readonly IBookAggregateRepository _bookAggregateRepository;

        public BookService(IBookAggregateRepository repository)
        {
            //TODO: Init Moderation Module
            _messagingService = new MessagingService();
            _bookAggregateRepository = repository;
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

            var bookAggregate = new BookAggregate(new BookState());
            var bookKey = bookAggregate.StartANewBook(author, language);
            BookAggregateCache.UpdateItem(bookKey, bookAggregate);
            _bookAggregateRepository.Save(bookAggregate);

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

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.AddAuthor(cmd.AuthorKey, newAuthor));
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

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.RemoveAuthor(cmd.AuthorKey, removeAuthor));
        }

        public void When(RetitleBookCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            if (string.IsNullOrEmpty(cmd.Title))
                cmd.Title = "Untitled";

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.Retitle(cmd.AuthorKey, cmd.Title));
        }

        public void When(ChangeBookSubtitleCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.ChangeSubTitle(cmd.AuthorKey, cmd.SubTitle));
        }

        public void When(ChangeBookSeriesTitleCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.ChangeSeriesTitle(cmd.AuthorKey, cmd.SeriesTitle));
        }

        public void When(ChangeBookSeriesVolumeCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.ChangeSeriesVolume(cmd.AuthorKey, cmd.SeriesVolume));
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

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.ChangeGenre(cmd.AuthorKey, newGenre));
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

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.ChangeEditorLanguage(cmd.AuthorKey, newLanguage));
        }

        public void When(ChangeBookSynopsisCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.UpdateSynopsis(cmd.AuthorKey, cmd.Synopsis));
        }

        public void When(CompleteBookCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.CompleteBook(cmd.AuthorKey));
        }

        public void When(SetBookInProgressCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.SetBookAsInProgress(cmd.AuthorKey));
        }

        public void When(AbandonBookCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.AbandonBook(cmd.AuthorKey));
        }

        public void When(ReviveBookCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.ReviveBook(cmd.AuthorKey));
        }

        public void When(PublishBookCommand cmd)
        {
            if (cmd.BookKey == null)
                throw new Exception("Book Key is required");

            if (cmd.AuthorKey == null)
                throw new Exception("Author Key is required");

            Act<BookAggregate>(cmd.BookKey, aggregate => aggregate.PublishBook(cmd.AuthorKey));
        }

        #region Private Methods

        private void Act<T>(Guid key, Action<T> action)
        {
            var aggregate = _bookAggregateRepository.FindByKey<T>(key);
            action(aggregate);
            _bookAggregateRepository.Save(aggregate);
        }

        #endregion
    }
}