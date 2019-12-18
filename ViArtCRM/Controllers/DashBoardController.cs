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

        public IActionResult Create(TaskObject task) {
            return View(task);
        }
        public IActionResult Edit(int id) {
            TasksContext context = HttpContext.RequestServices.GetService(typeof(TasksContext)) as TasksContext;
            return View(context.GetTaskByID(id));
        }

        [HttpPost]
        public ActionResult Move([FromBody]TaskMovingData data, int targetStatus = -1) {            
            TasksContext context = HttpContext.RequestServices.GetService(typeof(TasksContext)) as TasksContext;
            if (targetStatus == -1)
                targetStatus = Convert.ToInt32(data.currentTaskStatus) + 1;
            int rowsAffected = context.MoveTask(Convert.ToInt32(data.taskID), Convert.ToInt32(data.currentTaskStatus), targetStatus);

            return Json(String.Format("'RowsAffected':'{0}'", rowsAffected));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaskObject task) {
            TasksContext context = HttpContext.RequestServices.GetService(typeof(TasksContext)) as TasksContext;
            context.InsertTask(task);
            return RedirectToAction("Index");
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