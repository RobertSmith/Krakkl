using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Krakkl.Cache;
using Krakkl.Models;
using Krakkl.Query;
using Krakkl.Services;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;

namespace Krakkl.Controllers
{
    [Authorize]
    public class DeskController : Controller
    {
        public UserManager<ApplicationUser> UserManager { get; }
        private readonly DeskService _service = new DeskService();

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

        // GET: /Desk/NewBook
        [HttpGet]
        public async Task<ActionResult> NewBook()
        {
            ViewBag.Tab = "Desk";
            ViewBag.Genres = GenreCache.GetAll();
            ViewBag.Languages = LanguagesCache.GetAll();

            var user = await GetCurrentUserAsync();
            var newBookKey = _service.StartANewBook(user);

            return RedirectToAction("Book", "Desk", new { key = newBookKey });
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

            while (true)
            {
                try
                {
                    var book = await query.GetBookAsync(key);
                    return View(book);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("404"))
                        Thread.Sleep(1000);
                    else 
                        throw;
                }
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateBookTitle(string bookKey, string title)
        {
            var user = await GetCurrentUserAsync();

            if (_service.UpdateBookTitle(user, bookKey, title))
                return new JsonResult("success");

            return new JsonResult("Retitle failed");
        }

        [HttpPost]
        public async Task<JsonResult> UpdateBookSubTitle(string bookKey, string subTitle)
        {
            var user = await GetCurrentUserAsync();

            if (_service.UpdateBookSubTitle(user, bookKey, subTitle))
                return new JsonResult("success");

            return new JsonResult("Retitle failed");
        }

        [HttpPost]
        public async Task<JsonResult> UpdateBookSeriesTitle(string bookKey, string seriesTitle)
        {
            var user = await GetCurrentUserAsync();

            if (_service.UpdateBookSeriesTitle(user, bookKey, seriesTitle))
                return new JsonResult("success");

            return new JsonResult("Retitle failed");
        }

        [HttpPost]
        public async Task<JsonResult> UpdateBookSeriesVolume(string bookKey, string seriesVolume)
        {
            var user = await GetCurrentUserAsync();

            if (_service.UpdateBookSeriesVolume(user, bookKey, seriesVolume))
                return new JsonResult("success");

            return new JsonResult("Retitle failed");
        }

        [HttpPost]
        public async Task<JsonResult> UpdateBookGenre(string bookKey, string genre)
        {
            var user = await GetCurrentUserAsync();

            if (_service.UpdateBookGenre(user, bookKey, genre))
                return new JsonResult("success");

            return new JsonResult("Retitle failed");
        }

        [HttpPost]
        public async Task<JsonResult> UpdateBookLanguage(string bookKey, string language)
        {
            var user = await GetCurrentUserAsync();

            if (_service.UpdateBookLanguage(user, bookKey, language))
                return new JsonResult("success");

            return new JsonResult("Retitle failed");
        }

        [HttpPost]
        public async Task<JsonResult> UpdateSynopsis(string bookKey, string synopsis)
        {
            var user = await GetCurrentUserAsync();

            if (_service.UpdateSynopsis(user, bookKey, synopsis))
                return new JsonResult("success");

            return new JsonResult("Retitle failed");
        }

        [HttpPost]
        public async Task<JsonResult> UpdateBookPublished(string bookKey)
        {
            var user = await GetCurrentUserAsync();

            if (_service.UpdateBookPublished(user, bookKey))
                return new JsonResult("success");

            return new JsonResult("Retitle failed");
        }

        [HttpPost]
        public async Task<JsonResult> UpdateBookCompleted(string bookKey, bool completed)
        {
            var user = await GetCurrentUserAsync();

            if (_service.UpdateBookCompleted(user, bookKey, completed))
                return new JsonResult("success");

            return new JsonResult("Retitle failed");
        }

        [HttpPost]
        public async Task<JsonResult> UpdateBookAbandoned(string bookKey, bool abandoned)
        {
            var user = await GetCurrentUserAsync();

            if (_service.UpdateBookAbandoned(user, bookKey, abandoned))
                return new JsonResult("success");

            return new JsonResult("Retitle failed");
        }

        #region Helpers

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(Context.User.GetUserId());
        }

        #endregion
    }
}