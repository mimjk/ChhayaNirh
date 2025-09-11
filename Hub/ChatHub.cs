using ChhayaNirh.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChhayaNirh.Hubs
{
    public class ChatHub : Hub
    {
        private static Dictionary<int, string> connectedUsers = new Dictionary<int, string>();
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public async Task JoinChat(int userId, int receiverId)
        {
            var groupName = GetGroupName(userId, receiverId);
            await Groups.Add(Context.ConnectionId, groupName);

            // Store user connection
            connectedUsers[userId] = Context.ConnectionId;

            // Mark messages as delivered when user comes online
            await MarkMessagesAsDelivered(userId, receiverId);
        }

        public async Task LeaveChat(int userId, int receiverId)
        {
            var groupName = GetGroupName(userId, receiverId);
            await Groups.Remove(Context.ConnectionId, groupName);
        }

        public async Task UpdateMessageStatus(int messageId, string status)
        {
            try
            {
                var message = await db.Chats.FindAsync(messageId);
                if (message != null)
                {
                    switch (status.ToLower())
                    {
                        case "delivered":
                            if (!message.IsDelivered)
                            {
                                message.IsDelivered = true;
                                message.DeliveredAt = DateTime.Now;
                            }
                            break;
                        case "read":
                            message.IsRead = true;
                            message.ReadAt = DateTime.Now;
                            if (!message.IsDelivered)
                            {
                                message.IsDelivered = true;
                                message.DeliveredAt = DateTime.Now;
                            }
                            break;
                    }

                    await db.SaveChangesAsync();

                    var groupName = GetGroupName(message.SenderId, message.ReceiverId);

                    // Notify both users about status update
                    await Clients.Group(groupName).messageStatusUpdated(new
                    {
                        MessageId = messageId,
                        Status = message.MessageStatus,
                        SenderId = message.SenderId,
                        ReceiverId = message.ReceiverId
                    });
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.error("Failed to update message status: " + ex.Message);
            }
        }

        // Also modify your existing SendMessage method to return the actual message ID:
        public async Task SendMessage(int senderId, int receiverId, string messageText)
        {
            try
            {
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

                var groupName = GetGroupName(senderId, receiverId);

                // Check if receiver is online
                bool receiverOnline = connectedUsers.ContainsKey(receiverId);
                if (receiverOnline)
                {
                    // Mark as delivered immediately if receiver is online
                    message.IsDelivered = true;
                    message.DeliveredAt = DateTime.Now;
                    await db.SaveChangesAsync();
                }

                // Send message to both users with actual database ID
                await Clients.Group(groupName).messageReceived(new
                {
                    Id = message.Id, // This is now the actual database ID
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    MessageText = messageText,
                    SentAt = message.SentAt,
                    Status = message.MessageStatus
                });
            }
            catch (Exception ex)
            {
                // Handle error
                await Clients.Caller.error("Failed to send message: " + ex.Message);
            }
        }

        public async Task MarkAsRead(int userId, int senderId)
        {
            try
            {
                var unreadMessages = await db.Chats
                    .Where(c => c.SenderId == senderId && c.ReceiverId == userId && !c.IsRead)
                    .ToListAsync();

                foreach (var message in unreadMessages)
                {
                    message.IsRead = true;
                    message.ReadAt = DateTime.Now;
                    if (!message.IsDelivered)
                    {
                        message.IsDelivered = true;
                        message.DeliveredAt = DateTime.Now;
                    }
                }

                await db.SaveChangesAsync();

                var groupName = GetGroupName(userId, senderId);

                // Notify sender that messages were read
                await Clients.Group(groupName).messagesRead(new
                {
                    ReaderId = userId,
                    SenderId = senderId,
                    MessageIds = unreadMessages.Select(m => m.Id).ToList()
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.error("Failed to mark messages as read: " + ex.Message);
            }
        }

        private async Task MarkMessagesAsDelivered(int userId, int senderId)
        {
            try
            {
                var undeliveredMessages = await db.Chats
                    .Where(c => c.SenderId == senderId && c.ReceiverId == userId && !c.IsDelivered)
                    .ToListAsync();

                foreach (var message in undeliveredMessages)
                {
                    message.IsDelivered = true;
                    message.DeliveredAt = DateTime.Now;
                }

                if (undeliveredMessages.Any())
                {
                    await db.SaveChangesAsync();

                    var groupName = GetGroupName(userId, senderId);

                    // Notify sender that messages were delivered
                    await Clients.Group(groupName).messagesDelivered(new
                    {
                        ReceiverId = userId,
                        SenderId = senderId,
                        MessageIds = undeliveredMessages.Select(m => m.Id).ToList()
                    });
                }
            }
            catch (Exception ex)
            {
                // Log error
            }
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            // Remove user from connected users
            var userToRemove = connectedUsers.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (userToRemove.Key != 0)
            {
                connectedUsers.Remove(userToRemove.Key);
            }

            await base.OnDisconnected(stopCalled);
        }

        private string GetGroupName(int userId1, int userId2)
        {
            // Create consistent group name regardless of parameter order
            return userId1 < userId2 ? $"chat_{userId1}_{userId2}" : $"chat_{userId2}_{userId1}";
        }

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