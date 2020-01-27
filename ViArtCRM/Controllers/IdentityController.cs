using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViArtCRM.HelperTools;
using ViArtCRM.Models;


namespace ViArtCRM.Controllers {
    public class IdentityController : Controller {
        public IActionResult Index() {
            var context = HttpContext.RequestServices.GetService(typeof(UserContext)) as UserContext;
            return View("Index", context.GetUserByLogin(BrowserDataHelper.GetBrowserUserSession(Request.Cookies).Login));
        }

        public IActionResult Login() {
            return View("Login");
        }
        [HttpPost]
        public IActionResult Login(User user) {
            var cookiesSession = BrowserDataHelper.GetBrowserUserSession(Request.Cookies);
            if (cookiesSession != null)
                return Index();
            var userContext = HttpContext.RequestServices.GetService(typeof(UserContext)) as UserContext;
            var userData = userContext.GetUserByLoginAndPassword(user.Login, user.Password);
            if (userData == null) {
                ViewBag.login_error = "Incorrect username or password";
                return View(user);
            }
            var sessionContext = HttpContext.RequestServices.GetService(typeof(SessionContext)) as SessionContext;
            var userSession = sessionContext.CreateUserSession(user);
            BrowserDataHelper.WriteUserDataToCookies(userSession, Response.Cookies);
            return Index();
        }
    }
}