using System;
using System.Diagnostics;
using System.Linq;
using Krakkl.Authorship.Book.Service;
using Krakkl.Infrastructure.Authorship.Book.EventTranslator;
using Krakkl.Infrastructure.Authorship.Book.Repository;

namespace Krakkl.Console
{
    public class Program
    {
        private BookService _bookService;
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
//
//            var bookQuery = new Query.Books();
//            var books = bookQuery.GetAuthorBooksAsyc("85acdb07-9fa8-4fb2-983a-7b6d517c172f");
//
//            foreach (var book in books.Result)
//            {
//                System.Console.WriteLine(book.Title + " by " + book.Authors.First().Name);
//            }

//            var changeSynopsis = new ChangeBookSynopsisCommand
//            {
//                BookKey = Guid.Parse("5591cf0b-64d8-47d3-a823-c07a69bb6d07"),
//                AuthorKey = Guid.Parse("85acdb07-9fa8-4fb2-983a-7b6d517c172f"),
//                Synopsis = "A quaint little story about a boy and his llama."
//            };
//                        
//            _bookService.Apply(changeSynopsis);

            var changeSynopsis = new ChangeBookSubtitleCommand
            {
                BookKey = Guid.Parse("5591cf0b-64d8-47d3-a823-c07a69bb6d07"),
                AuthorKey = Guid.Parse("85acdb07-9fa8-4fb2-983a-7b6d517c172f"),
                SubTitle = "Radar love in the preindustrial era."
            };
                                    
            _bookService.Apply(changeSynopsis);

            timer.Stop();

            System.Console.WriteLine("Done in " + timer.ElapsedMilliseconds + "ms");
            System.Console.ReadLine();
        }
    }
}