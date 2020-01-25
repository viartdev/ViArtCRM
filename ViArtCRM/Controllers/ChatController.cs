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
            return View();
        }

   
        public IActionResult LoadGroup()
        {
            ChatContext context = HttpContext.RequestServices.GetService(typeof(ChatContext)) as ChatContext;
            var groups = context.GetGroupsContainer();

            return PartialView("ChatGroup", groups);
        }

        public IActionResult LoadMessages(int groupID)
        {
            ChatContext context = HttpContext.RequestServices.GetService(typeof(ChatContext)) as ChatContext;
            var messages = context.getMessagesByGroupID(groupID);

            return PartialView("ChatMessages", messages);
        }

    }
}