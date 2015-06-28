using System.Security.Claims;
using System.Threading.Tasks;
using Krakkl.Models;
using Krakkl.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;

namespace Krakkl.Controllers
{
    public class ImageController : Controller
    {
        public UserManager<ApplicationUser> UserManager { get; }
        private readonly ImageService _service = new ImageService();

        public ImageController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        // GET: /Image/Browser
        public async Task<ActionResult> Browser(string bookKey)
        {
            ViewBag.CKEditorFuncNum = Request.Query["CKEditorFuncNum"];

            var user = await UserManager.FindByIdAsync(Context.User.GetUserId());
            return View(_service.GetBookImageUrls(bookKey));
        }

        //
        // POST: /Image/BrowserSelect
        [HttpPost]
        public async Task<ActionResult> BrowserSelect(string url)
        {
            var message = string.Empty;
            var user = await UserManager.FindByIdAsync(Context.User.GetUserId());

            if (string.IsNullOrEmpty(url))
                return Content(string.Empty);

            return Content("<script type='text/javascript'>window.parent.CKEDITOR.tools.callFunction(" + Request.Form["CKEditorFuncNum"] + ", '" + url + "', '" + message + "');</script>");
        }
    }
}
