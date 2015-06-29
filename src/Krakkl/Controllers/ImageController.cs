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

        //
        // POST: /Image/Upload
        //        [HttpPost]
        //        public async Task<ActionResult> Upload(string bookId)
        //        {
        //            var message = string.Empty;
        //            var url = string.Empty;
        //
        //            try
        //            {
        //                var user = await UserManager.FindByIdAsync(Context.User.GetUserId());
        //                var image = Request.Form.Files[0].OpenReadStream();
        //                url = _service.AddImage(bookId, image);
        //            }
        //            catch (Exception ex)
        //            {
        //                message = ex.Message;
        //            }
        //
        //            return Content("<script type='text/javascript'>window.parent.CKEDITOR.tools.callFunction(" + Request.QueryString["CKEditorFuncNum"] + ", '" + url + "', '" + message + "');</script>");
        //        }

        // GET: /Image/Browser
        [HttpGet]
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
