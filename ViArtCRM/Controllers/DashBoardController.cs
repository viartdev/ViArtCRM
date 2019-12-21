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

        [HttpGet, ActionName("LoadModule")]
        public ActionResult LoadModule(int taskStatus, int moduleID) {
            TasksContext context = HttpContext.RequestServices.GetService(typeof(TasksContext)) as TasksContext;
            var tasks = context.GetTasks(taskStatus, moduleID);
            if (taskStatus == 0)
                return PartialView("ToDoHolder", tasks);
            else if (taskStatus == 1)
                return PartialView("ModerateHolder", tasks);
            else
                return PartialView("CompleteHolder", tasks);
        }


        public IActionResult Create(TaskObject task) {
            return View(task);
        }
        public IActionResult Edit(TaskObject task) {
            TasksContext context = HttpContext.RequestServices.GetService(typeof(TasksContext)) as TasksContext;
            return View(context.GetTaskByID(task.TaskID));
        }

        [HttpPost]
        public ActionResult Move([FromBody]TaskMovingData data, int targetStatus = -1) {
            TasksContext context = HttpContext.RequestServices.GetService(typeof(TasksContext)) as TasksContext;
            if (targetStatus == -1)
                targetStatus = Convert.ToInt32(data.currentTaskStatus) + 1;
            int rowsAffected = context.MoveTask(Convert.ToInt32(data.taskID), Convert.ToInt32(data.currentTaskStatus), targetStatus);

            return Json(String.Format("{{\"success\":\"{0}\"}}", rowsAffected > 0 ? "ok" : "no"));
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditMethod(TaskObject task) {
            TasksContext context = HttpContext.RequestServices.GetService(typeof(TasksContext)) as TasksContext;
            context.UpdateTask(task);
            return RedirectToAction("Index", new { moduleID = task.ModuleID });
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMethod(TaskObject task) {
            TasksContext context = HttpContext.RequestServices.GetService(typeof(TasksContext)) as TasksContext;
            context.InsertTask(task);
            return RedirectToAction("Index", new { moduleID = task.ModuleID });
        }
    }
}