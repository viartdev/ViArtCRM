using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ViArtCRM.Models;

namespace ViArtCRM.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            ChatContext context = HttpContext.RequestServices.GetService(typeof(ChatContext)) as ChatContext;
            return View(context.GetGroupsContainer());
        }
    }
}