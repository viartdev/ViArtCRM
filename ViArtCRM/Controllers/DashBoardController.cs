using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViArtCRM.Models;

namespace ViArtCRM.Controllers {
    public class DashBoardController : Controller {
        
        [Route("DashBoard/Index/{moduleid?}")]
        public IActionResult Index(int moduleid) {            
            if (moduleid == -1)
                return RedirectToAction("Error");

            TasksContext context = HttpContext.RequestServices.GetService(typeof(TasksContext)) as TasksContext;
            return View(context.GetTaskContainer(moduleid));
        }

        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.Task task) {
            TasksContext context = HttpContext.RequestServices.GetService(typeof(TasksContext)) as TasksContext;
            context.InsertTask(task);
            return RedirectToAction("Index");
        }
    }
}