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
            int userID = 1; //USER ID
            ViewBag.userID = userID;
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

        [HttpPost]
        public ActionResult SendMessage([FromBody]MessageMovingData data)
        {
            ChatContext context = HttpContext.RequestServices.GetService(typeof(ChatContext)) as ChatContext;
            int rowsAffected = 1;
            context.insertMessage(data.userID, data.groupID, data.messageText);
            return Json(String.Format("{{\"success\":\"{0}\"}}", rowsAffected > 0 ? "ok" : "no"));
        }
        

        [HttpPost]
        public ActionResult RefreshGroupLastMessage([FromBody]RefreshGroupMovingData data)
        {
            ChatContext context = HttpContext.RequestServices.GetService(typeof(ChatContext)) as ChatContext;
            int rowsAffected = 1;
            context.RefreshGroupLastMessage(data.messageID, data.groupID);
            return Json(String.Format("{{\"success\":\"{0}\"}}", rowsAffected > 0 ? "ok" : "no"));
        }

    }
}