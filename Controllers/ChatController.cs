using ChhayaNirh.Models;
using ChhayaNirh.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ChhayaNirh.Controllers
{
    public class ChatController : Controller
    {
        // Add database context
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Chat - Shows list of all conversations
        public async Task<ActionResult> Chat()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int currentUserId = Convert.ToInt32(Session["UserId"]);

            var chatList = await db.Chats
                .Where(c => c.SenderId == currentUserId || c.ReceiverId == currentUserId)
                .GroupBy(c => c.SenderId == currentUserId ? c.ReceiverId : c.SenderId)
                .Select(g => new {
                    UserId = g.Key,
                    LastMessage = g.OrderByDescending(c => c.SentAt).FirstOrDefault()
                })
                .ToListAsync();

            var chatViewModels = new List<ChatListViewModel>();

            foreach (var chat in chatList)
            {
                var user = await db.Users.FindAsync(chat.UserId);
                if (user != null && chat.LastMessage != null)
                {
                    chatViewModels.Add(new ChatListViewModel
                    {
                        UserId = chat.UserId,
                        UserName = user.FullName,
                        LastMessageText = chat.LastMessage.MessageText,
                        LastMessageTime = chat.LastMessage.SentAt,
                        ProfilePicturePath = string.IsNullOrEmpty(user.ProfilePicturePath)
                    ? "~/Content/Images/default-profile2.png"
                    : user.ProfilePicturePath
                    });
                }
            }

            return View(chatViewModels.OrderByDescending(c => c.LastMessageTime).ToList());
        }

        // GET: Inbox - Shows conversation with specific user
        public async Task<ActionResult> Inbox(int userId)
        {
            // Use Session instead of User.Identity for your project
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int currentUserId = Convert.ToInt32(Session["UserId"]);

            var messages = await db.Chats
                .Where(c => (c.SenderId == currentUserId && c.ReceiverId == userId) ||
                            (c.SenderId == userId && c.ReceiverId == currentUserId))
                .OrderBy(c => c.SentAt)
                .ToListAsync();

            ViewBag.ReceiverId = userId;

            // Get receiver's name for display
            var receiver = await db.Users.FindAsync(userId);
            ViewBag.ReceiverName = receiver?.FullName ?? "Unknown User";

            return View(messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendMessage(int ReceiverId, string MessageText)
        {
            if (string.IsNullOrWhiteSpace(MessageText))
            {
                return RedirectToAction("Inbox", new { userId = ReceiverId });
            }

            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int senderId = Convert.ToInt32(Session["UserId"]);

            Chat newMessage = new Chat
            {
                SenderId = senderId,
                ReceiverId = ReceiverId,
                MessageText = MessageText,
                SentAt = DateTime.Now,
                IsRead = false
            };

            db.Chats.Add(newMessage);
            await db.SaveChangesAsync();

            return RedirectToAction("Inbox", new { userId = ReceiverId });
        }

        // Dispose database context
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}