using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ViArtCRM.Models;


namespace ViArtCRM.Controllers
{
    public class IdentityController : Controller
    {
        public IActionResult Index()
        {
            var context = HttpContext.RequestServices.GetService(typeof(UserContext)) as UserContext;
            return View(context.GetUsers());
        }
    }
}