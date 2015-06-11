﻿using System;
using System.Diagnostics;
using Krakkl.Authorship.Service;
using Krakkl.Persistence.Authorship;

namespace Krakkl.Console
{
    public class Program
    {
        private BookService _bookService;
        private AuthorshipService _authorshipService;

        public void Main(string[] args)
        {
            var authorKey = Guid.NewGuid();
            var newAuthorKey = Guid.NewGuid();
            _bookService = new BookService();
            _authorshipService = new AuthorshipService();

            var timer = new Stopwatch();
            timer.Start();

//            var bookKey = _bookService.StartANewBook(authorKey, "Joe Shmoe", "en", "English");
//            _bookService.AddAuthorToBook(bookKey, authorKey, newAuthorKey, "Jimmy the Greek");
//            _bookService.RemoveAuthorFromBook(bookKey, newAuthorKey, authorKey, "Joe Shmoe");
//            _bookService.ChangeBookGenre(bookKey, newAuthorKey, "ActionAdventure", "Action & Adventure", true);

            timer.Stop();

            System.Console.WriteLine("Done in " + timer.ElapsedMilliseconds + "ms");
            System.Console.ReadLine();
        }
    }
}