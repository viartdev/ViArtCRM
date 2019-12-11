using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ViArtCRM.Models;

namespace ViArtCRM.Controllers {
    public class ModulesController : Controller {
        public IActionResult Index() {
            TaskModuleContext context = HttpContext.RequestServices.GetService(typeof(TaskModuleContext)) as TaskModuleContext;
            return View(context.GetModules());
        }
    }
}