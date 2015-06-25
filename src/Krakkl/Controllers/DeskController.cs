using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Krakkl.Authorship.Services;
using Krakkl.Cache;
using Krakkl.Models;
using Krakkl.Query;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;

namespace Krakkl.Controllers
{
    [Authorize]
    public class DeskController : Controller
    {
        public UserManager<ApplicationUser> UserManager { get; }

        public DeskController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        // GET: /Desk
        public async Task<ActionResult> Index()
        {
            ViewBag.Tab = "Desk";
            var query = new Books();
            var user = await GetCurrentUserAsync();
            var books = await query.GetAuthorBooksAsyc(user.Id);

            return View(books);
        }

        // POST: /Desk/NewBook
        [HttpPost]
        public async Task<ActionResult> NewBook(string message)
        {
            ViewBag.Tab = "Desk";
            ViewBag.Genres = GenreCache.GetAll();
            ViewBag.Languages = LanguagesCache.GetAll();

            if (!string.IsNullOrEmpty(message))
                ViewBag.StatusMessage = message;

            var user = await GetCurrentUserAsync();
            var languages = LanguagesCache.GetAll() as Dictionary<string, string>;

            var bookService = new BookService(new Authorship.Infrastructure.BookAggregateRepository());
            var bookKey = bookService.Start(new StartANewBookCommand
            {
                AuthorKey = Guid.Parse(user.Id),
                AuthorName = user.Pseudonym,
                LanguageKey = languages?.First(x => x.Key == user.EditorLanguage).Key,
                LanguageName = languages?.First(x => x.Key == user.EditorLanguage).Value
            });

            return RedirectToAction("Book", "Desk", new { key = bookKey });
        }

        // GET: /Desk/Book
        public async Task<ActionResult> Book(string key, string message)
        {
            ViewBag.Key = key;
            ViewBag.Tab = "Desk";
            ViewBag.SubTab = "Book";
            ViewBag.Genres = GenreCache.GetAll();
            ViewBag.Languages = LanguagesCache.GetAll();

            if (!string.IsNullOrEmpty(message))
                ViewBag.StatusMessage = message;

            var query = new Books();
            var book = await query.GetBookAsync(key);

            return View(book);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateBookTitle(string bookKey, string title)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var bookService = new BookService(new Authorship.Infrastructure.BookAggregateRepository());

                bookService.Apply(new RetitleBookCommand
                {
                    AuthorKey = Guid.Parse(user.Id),
                    BookKey = Guid.Parse(bookKey),
                    Title = title
                });
            }
            catch (Exception)
            {
                return new JsonResult("Retitle failed");
            }

            return new JsonResult("success");
        }

        #region Helpers

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(Context.User.GetUserId());
        }

        #endregion
    }
}