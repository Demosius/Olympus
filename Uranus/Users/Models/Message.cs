using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Users.Models;

public class Message
{
    [PrimaryKey] public Guid ID { get; set; }
    [ForeignKey(typeof(User))] public int UserID { get; set; }
    [ForeignKey(typeof(Conversation))] public Guid ConversationID { get; set; }
    public DateTime DateTime { get; set; }
    public string Text { get; set; }

    [ManyToOne(nameof(ConversationID), nameof(Models.Conversation.Messages), CascadeOperations = CascadeOperation.CascadeRead)]
    public Conversation? Conversation { get; set; }
    [ManyToOne(nameof(UserID), nameof(Models.User.Messages), CascadeOperations = CascadeOperation.CascadeRead)]
    public User? User { get; set; }

    public Message()
    {
        ID = Guid.NewGuid();
        Text = string.Empty;
    }

    public Message(int userID, Guid conversationID, DateTime dateTime) : this()
    {
        UserID = userID;
        ConversationID = conversationID;
        DateTime = dateTime;
    }

    public Message(User user, Conversation conversation, DateTime dateTime) : this(user.ID, conversation.ID, dateTime)
    {
        Conversation = conversation;
        User = user;
    }
}