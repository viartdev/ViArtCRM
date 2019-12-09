using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ViArtCRM.Models;

namespace ViArtCRM.Controllers
{
    public class SchedulerController : Controller
    {

        public IActionResult Index()
        {
            TasksContext context = HttpContext.RequestServices.GetService(typeof(TasksContext)) as TasksContext;
            return View(context.GetTasks());
        }
    }
}