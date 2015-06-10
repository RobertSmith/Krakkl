using System;
using Krakkl.Authorship.Service;

namespace Krakkl.Console
{
    public class Program
    {
        private BookService _bookService;

        public void Main(string[] args)
        {
            var authorKey = Guid.NewGuid();
            var newAuthorKey = Guid.NewGuid();
            _bookService = new BookService();

            var bookKey = _bookService.StartANewBook(authorKey, "Joe Shmoe", "en", "English");
            _bookService.AddAuthorToBook(bookKey, authorKey, newAuthorKey, "Jimmy the Greek");
            _bookService.RemoveAuthorFromBook(bookKey, newAuthorKey, authorKey, "Joe Shmoe");

            System.Console.WriteLine("Done");
            System.Console.ReadLine();
        }
    }
}