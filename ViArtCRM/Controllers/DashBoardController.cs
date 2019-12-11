using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ViArtCRM.Models;

namespace ViArtCRM.Controllers {
    public class DashBoardController : Controller {

        public string GetConvertedValue(byte[] sessionValue) {
            return System.Text.Encoding.UTF8.GetString(sessionValue);
        }

        int GetModuleID() {
            byte[] moduleID = new byte[20];
            if (HttpContext.Session.TryGetValue("moduleid", out moduleID))
                return Convert.ToInt32(GetConvertedValue(moduleID));
            else
                return -1;
        }
        public IActionResult Index() {
            var moduleID = GetModuleID();
            if (moduleID == -1)
                return RedirectToAction("Error");

            TasksContext context = HttpContext.RequestServices.GetService(typeof(TasksContext)) as TasksContext;
            return View(context.GetTaskContainer(moduleID));
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