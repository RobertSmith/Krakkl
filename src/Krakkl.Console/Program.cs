﻿using System;
using System.Diagnostics;
using Krakkl.Authorship.Repository;
using Krakkl.Authorship.Service;
using Krakkl.EventTranslator;
using Krakkl.Persistence.Authorship;

namespace Krakkl.Console
{
    public class Program
    {
        private BookService _bookService;
        private AuthorshipService _authorshipService = new AuthorshipService(); // spin up message handler
        private BookEventTranslator _bookEventTranslator = new BookEventTranslator(); // spin up translator

        public void Main(string[] args)
        {
            var authorKey = Guid.NewGuid();
            var newAuthorKey = Guid.NewGuid();
            _bookService = new BookService(new BookAggregateRepository());

            var timer = new Stopwatch();
            timer.Start();

//            var startANewBook = new StartANewBookCommand
//            {
//                AuthorKey = authorKey,
//                AuthorName = "Joe Shmoe",
//                LanguageKey = "en",
//                LanguageName = "English"
//            };
//
//            var bookKey = _bookService.Start(startANewBook);
//
//            var addAuthor = new AddAuthorToBookCommand
//            {
//                BookKey = bookKey,
//                AuthorKey = authorKey,
//                NewAuthorKey = newAuthorKey,
//                NewAuthorName = "Jimmy the Greek"
//            };
//
//            _bookService.Apply(addAuthor);
//
//            var removeAuthor = new RemoveAuthorFromBookCommand
//            {
//                BookKey = bookKey,
//                AuthorKey = newAuthorKey,
//                RemoveAuthorKey = authorKey,
//                RemoveAuthorName = "Joe Shmoe"
//            };
//
//            _bookService.Apply(removeAuthor);
//
//            var changeGenre = new ChangeBookGenreCommand
//            {
//                BookKey = bookKey,
//                AuthorKey = newAuthorKey,
//                GenreKey = "ActionAdventure",
//                GenreName = "Action & Adventure",
//                IsFiction = true
//            };
//
//            _bookService.Apply(changeGenre);
//
//            var changeTitle = new RetitleBookCommand
//            {
//                BookKey = bookKey,
//                AuthorKey = newAuthorKey,
//                Title = "The End of all Things"
//            };
//
//            _bookService.Apply(changeTitle);

            var changeSynopsis = new ChangeBookSynopsisCommand
            {
                BookKey = Guid.Parse("7cbb2741-d8e4-4d93-8550-7ab5d0675be3"),
                AuthorKey = Guid.Parse("bc76debb-cf3f-4f17-b40a-0f59adae85d4"),
                Synopsis = "A quaint little story about a boy and his llama."
            };

            _bookService.Apply(changeSynopsis);

            timer.Stop();

            System.Console.WriteLine("Done in " + timer.ElapsedMilliseconds + "ms");
            System.Console.ReadLine();
        }
    }
}