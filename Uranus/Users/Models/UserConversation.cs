using System;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Users.Models;

public class UserConversation
{
    [ForeignKey(typeof(User))] public int UserID { get; set; }
    [ForeignKey(typeof(Conversation))] public Guid ConversationID { get; set; }
}