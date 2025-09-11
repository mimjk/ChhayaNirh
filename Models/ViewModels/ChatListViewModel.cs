using System;

namespace ChhayaNirh.ViewModels
{
    public class ChatListViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string LastMessageText { get; set; }
        public DateTime LastMessageTime { get; set; }
        public string ProfilePicturePath { get; set; }
    }
}