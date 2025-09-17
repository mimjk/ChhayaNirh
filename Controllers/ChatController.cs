using ChhayaNirh.Models;
using ChhayaNirh.ViewModels;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ChhayaNirh.Hubs;

namespace ChhayaNirh.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        private int GetAdminId()
        {
            // Change this if your Admin user Id changes in DB
            return 3;
        }

        // GET: Chat - list of all conversations
        public async Task<ActionResult> Chat()
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Account");

            int currentUserId = Convert.ToInt32(Session["UserId"]);

            var chatList = await db.Chats
                .Where(c => c.SenderId == currentUserId || c.ReceiverId == currentUserId)
                .GroupBy(c => c.SenderId == currentUserId ? c.ReceiverId : c.SenderId)
                .Select(g => new {
                    UserId = g.Key,
                    LastMessage = g.OrderByDescending(c => c.SentAt).FirstOrDefault(),
                    UnreadCount = g.Count(c => c.ReceiverId == currentUserId && !c.IsRead)
                })
                .ToListAsync();

            var chatViewModels = new List<ChatListViewModel>();

            foreach (var chat in chatList)
            {
                string userName;
                string profilePicturePath;

                if (chat.UserId == GetAdminId()) // Admin logic
                {
                    userName = "Admin";
                    profilePicturePath = "~/Content/Images/admin-avatar.png";
                }
                else
                {
                    var user = await db.Users.FindAsync(chat.UserId);
                    if (user == null) continue;

                    userName = user.FullName;
                    profilePicturePath = string.IsNullOrEmpty(user.ProfilePicturePath)
                        ? "~/Content/Images/default-profile2.png"
                        : user.ProfilePicturePath;
                }

                if (chat.LastMessage != null)
                {
                    chatViewModels.Add(new ChatListViewModel
                    {
                        UserId = chat.UserId,
                        UserName = userName,
                        LastMessageText = chat.LastMessage.MessageText,
                        LastMessageTime = chat.LastMessage.SentAt,
                        ProfilePicturePath = profilePicturePath,
                        UnreadCount = chat.UnreadCount
                    });
                }
            }

            return View(chatViewModels.OrderByDescending(c => c.LastMessageTime).ToList());
        }

        // GET: Inbox
        public async Task<ActionResult> Inbox(int userId)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Account");

            int currentUserId = Convert.ToInt32(Session["UserId"]);

            var messages = await db.Chats
                .Where(c => (c.SenderId == currentUserId && c.ReceiverId == userId) ||
                            (c.SenderId == userId && c.ReceiverId == currentUserId))
                .OrderBy(c => c.SentAt)
                .ToListAsync();

            // Mark unread messages as read
            var unreadMessages = messages.Where(m => m.SenderId == userId && !m.IsRead).ToList();
            foreach (var msg in unreadMessages)
            {
                msg.IsRead = true;
                msg.ReadAt = DateTime.Now;
                if (!msg.IsDelivered)
                {
                    msg.IsDelivered = true;
                    msg.DeliveredAt = DateTime.Now;
                }
            }

            if (unreadMessages.Any())
            {
                await db.SaveChangesAsync();
                var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                var groupName = GetGroupName(currentUserId, userId);
                await context.Clients.Group(groupName).messagesRead(new
                {
                    ReaderId = currentUserId,
                    SenderId = userId,
                    MessageIds = unreadMessages.Select(m => m.Id).ToList()
                });
            }

            ViewBag.ReceiverId = userId;
            ViewBag.CurrentUserId = currentUserId;

            ViewBag.ReceiverName = (userId == GetAdminId())
                ? "Admin"
                : (await db.Users.FindAsync(userId))?.FullName ?? "Unknown User";

            return View(messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendMessage(int ReceiverId, string MessageText)
        {
            if (string.IsNullOrWhiteSpace(MessageText)) return RedirectToAction("Inbox", new { userId = ReceiverId });
            if (Session["UserId"] == null) return RedirectToAction("Login", "Account");

            int senderId = Convert.ToInt32(Session["UserId"]);

            // Replace AdminId dynamically if sending as Admin
            if (ReceiverId == GetAdminId()) return RedirectToAction("Inbox", new { userId = ReceiverId });

            var message = new Chat
            {
                SenderId = senderId,
                ReceiverId = ReceiverId,
                MessageText = MessageText,
                SentAt = DateTime.Now,
                IsDelivered = false,
                IsRead = false
            };

            db.Chats.Add(message);
            await db.SaveChangesAsync();

            return RedirectToAction("Inbox", new { userId = ReceiverId });
        }

        [HttpPost]
        public async Task<JsonResult> SendMessageApi(int receiverId, string messageText)
        {
            if (string.IsNullOrWhiteSpace(messageText))
                return Json(new { success = false, message = "Message cannot be empty" });

            if (Session["UserId"] == null)
                return Json(new { success = false, message = "Not authenticated" });

            try
            {
                int senderId = Convert.ToInt32(Session["UserId"]);

                // Use actual Admin Id, cannot send messages to Admin via API
                if (receiverId == GetAdminId())
                    return Json(new { success = false, message = "Cannot send messages to Admin." });

                var message = new Chat
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    MessageText = messageText,
                    SentAt = DateTime.Now,
                    IsDelivered = false,
                    IsRead = false
                };

                db.Chats.Add(message);
                await db.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    messageId = message.Id,
                    sentAt = message.SentAt,
                    status = message.MessageStatus
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to send message: " + ex.Message });
            }
        }

        private string GetGroupName(int userId1, int userId2)
        {
            return userId1 < userId2 ? $"chat_{userId1}_{userId2}" : $"chat_{userId2}_{userId1}";
        }

        [HttpGet]
        public async Task<JsonResult> GetUnreadMessageCount()
        {
            if (Session["UserId"] == null) return Json(new { count = 0 }, JsonRequestBehavior.AllowGet);

            int currentUserId = Convert.ToInt32(Session["UserId"]);
            var unreadCount = await db.Chats.CountAsync(c => c.ReceiverId == currentUserId && !c.IsRead);
            return Json(new { count = unreadCount }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db?.Dispose();
            base.Dispose(disposing);
        }
    }
}
