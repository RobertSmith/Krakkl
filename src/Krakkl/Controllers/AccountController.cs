﻿using System.Security.Claims;
using System.Threading.Tasks;
using Krakkl.Cache;
using Krakkl.Models;
using Krakkl.Query;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;

namespace Krakkl.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public UserManager<ApplicationUser> UserManager { get; }
        public SignInManager<ApplicationUser> SignInManager { get; }

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set shouldLockout: true
                var result =
                    await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                    return RedirectToLocal(returnUrl);

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                return View(model);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {UserName = model.Email, Email = model.Email};
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, false);

                    return RedirectToAction("Index", "Home");
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogOff()
        {
            SignInManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new {ReturnUrl = returnUrl});
            var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var info = await SignInManager.GetExternalLoginInfoAsync();

            if (info == null)
                return RedirectToAction("Login");

            // Sign in the user with this external login provider if the user already has a login.
            var result = await SignInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

            if (result.Succeeded)
                return RedirectToLocal(returnUrl);

            // If the user does not have an account, then ask the user to create an account.
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.LoginProvider = info.LoginProvider;
            var email = info.ExternalPrincipal.FindFirstValue(ClaimTypes.Email);

            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel {Email = email});
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model,
            string returnUrl = null)
        {
            if (User.IsSignedIn())
                return RedirectToAction("Index", "Manage");

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await SignInManager.GetExternalLoginInfoAsync();

                if (info == null)
                    return View("ExternalLoginFailure");

                var user = new ApplicationUser {UserName = model.Email, Email = model.Email};
                var result = await UserManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user, info);

                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, false);
                        return RedirectToLocal(returnUrl);
                    }
                }

                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }

        // GET: /Account/Desk
        public async Task<ActionResult> Desk()
        {
            ViewBag.Tab = "Desk";
            var query = new Books();
            var user = await GetCurrentUserAsync();
            var books = query.GetAuthorBooks(user.Id);

            return View(books);
        }

        // GET: /Account/NewBook
        public async Task<ActionResult> NewBook(string message)
        {
            ViewBag.Tab = "Desk";
            ViewBag.Genres = GenreCache.GetAll();
            ViewBag.Languages = LanguagesCache.GetAll();

            if (!string.IsNullOrEmpty(message))
                ViewBag.StatusMessage = message;

            var user = await GetCurrentUserAsync();
            var newBook = new BookModel { AuthorName = user.UserName , LanguageKey = user.EditorLanguage };

            return View(newBook);
        }

//        [HttpPost]
//        public async Task<ActionResult> NewBook(NewBookModel model)
//        {
//            var coverArt = Request.Form.Files["CoverArt"];
//            var user = await GetCurrentUserAsync();
//
//            var genres = GenreCache.GetAll() as List<Query.GenreObject>;
//            var languages = LanguagesCache.GetAll() as Dictionary<string, string>;
//
//            var newBook = new Command.NewBookCommand(user.Id, user.Pseudonym, model.Title, model.GenreKey,
//                genres?.Single(x => x.Key == model.GenreKey).Name, model.LanguageKey,
//                languages?.Single(x => x.Key == model.LanguageKey).Value, model.Public)
//            {
//                SeriesTitle = model.SeriesTitle,
//                SeriesVolume = model.SeriesVolume,
//                SubTitle = model.SubTitle,
//                Synopsis = model.Synopsis
//            };
//
//            var key = newBook.Commit();
//
//            return RedirectToAction("Desk");
//        }

        // GET: /Account/BookMeta
        public async Task<ActionResult> BookMeta(string key, string message)
        {
            ViewBag.Key = key;
            ViewBag.Tab = "Desk";
            ViewBag.SubTab = "Meta";
            ViewBag.Genres = GenreCache.GetAll();
            ViewBag.Languages = LanguagesCache.GetAll();

            if (!string.IsNullOrEmpty(message))
                ViewBag.StatusMessage = message;

            var query = new Books();
            var book = await query.GetBookAsync(key);

            return View(book);
        }

//        // POST: /Account/BookMeta
//        [HttpPost]
//        public async Task<ActionResult> BookMeta(EditBookMetaModel model)
//        {
//            var coverArt = Request.Form.Files["CoverArt"];
//            var user = await GetCurrentUserAsync();
//
//            var genres = GenreCache.GetAll() as List<Query.GenreObject>;
//            var languages = LanguagesCache.GetAll() as Dictionary<string, string>;
//
//            var updateBook = new Command.UpdateBookCommand(model.Key, user.Id)
//            {
//                Title = model.Title,
//                SubTitle = model.SubTitle,
//                SeriesTitle = model.SeriesTitle,
//                SeriesVolume = model.SeriesVolume,
//                GenreKey = model.GenreKey,
//                Genre = genres?.Single(x => x.Key == model.GenreKey).Name,
//                LanguageKey = model.LanguageKey,
//                Language = languages?.Single(x => x.Key == model.LanguageKey).Value,
//                Synopsis = model.Synopsis,
//                Public = model.Public,
//                Complete = model.Complete,
//                Abandoned = model.Abandoned
//            };
//
//            updateBook.Commit();
//
//            return RedirectToAction("Desk");
//        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(Context.User.GetUserId());
        }

        #endregion
    }
}