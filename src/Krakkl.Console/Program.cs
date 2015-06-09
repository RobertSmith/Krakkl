using System;
using System.Threading;
using Krakkl.Authorship.Service;

namespace Krakkl.Console
{
    public class Program
    {
        private BookService _bookService;
        public void Main(string[] args)
        {
            var authorKey = Guid.NewGuid();

            _bookService = new BookService();

            var bookKey = _bookService.StartANewBook(authorKey, "Test Author", "en", "English");

            _bookService.RetitleBook(bookKey, authorKey, "This is an awesome book, says everyone!");

            System.Console.WriteLine(bookKey);
            System.Console.WriteLine("Sleeping now.");

            Thread.Sleep(35000);

            System.Console.WriteLine("Woke up");

            _bookService.RetitleBook(bookKey, authorKey, "This is an awesome book, said no one, ever!");

            System.Console.ReadLine();
        }
    }
}
