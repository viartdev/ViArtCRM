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
            
            int i = 0;
            foreach(var task in tasks)
            {
                if(task.TaskID == 3)
                {
                    task.TaskProgress = 10;
                }
                double nCompleted = 0;
                double count = 0;
                List<SubTask> subTasks = context.GetSubTasks(task.TaskID);
                foreach(var subtask in subTasks)
                {
                    if(subtask.isComplete == 1)
                    {
                        nCompleted += 1;
                    }
                    count += 1;
                }
                double progress = 0;
                if(count != 0) {
                    double procents = nCompleted / count;
                    progress = procents * 100;
                }
                var progressRounded = (int)Math.Round(progress);
                tasks[i].TaskProgress = progressRounded;
                i += 1;
            }
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
        public IActionResult Edit(int taskID) {
            TasksContext context = HttpContext.RequestServices.GetService(typeof(TasksContext)) as TasksContext;
            return PartialView(context.GetTaskByID(taskID));
        }
        
        public IActionResult ToDoSubTasks(int taskID)
        {
            TasksContext context = HttpContext.RequestServices.GetService(typeof(TasksContext)) as TasksContext;
            var list = context.GetSubTasks(taskID);
            if (list.Count == 0) {
                list.Add(new SubTask {
                    TaskID = taskID
                });
                return PartialView("EmptySubTasks", list);
            }
            return PartialView(context.GetSubTasks(taskID));
        }

        public IActionResult ModerateSubTasks(int taskID) {
            TasksContext context = HttpContext.RequestServices.GetService(typeof(TasksContext)) as TasksContext;
            var list = context.GetSubTasks(taskID);
            if (list.Count == 0)
            {
                list.Add(new SubTask
                {
                    TaskID = taskID
                });
                return PartialView("EmptySubTasks", list);
            }
            return PartialView(context.GetSubTasks(taskID));
        }

        [HttpPost]
        public ActionResult Move([FromBody]TaskMovingData data, int targetStatus = -1) {
            TasksContext context = HttpContext.RequestServices.GetService(typeof(TasksContext)) as TasksContext;
            if (targetStatus == -1)
                targetStatus = Convert.ToInt32(data.currentTaskStatus) + 1;
            int rowsAffected = 1;
            context.MoveTask(Convert.ToInt32(data.taskID), Convert.ToInt32(data.currentTaskStatus), targetStatus);

            return Json(String.Format("{{\"success\":\"{0}\"}}", rowsAffected > 0 ? "ok" : "no"));
        }

        [HttpPost]
        public ActionResult UpdateSubTask([FromBody]SubTaskMovingData data)
        {
            TasksContext context = HttpContext.RequestServices.GetService(typeof(TasksContext)) as TasksContext;
            int rowsAffected = 1;
            context.UpdateSubTask(Convert.ToInt32(data.taskID), data.SubTaskName);

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