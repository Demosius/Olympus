using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Users.Models;

public class Conversation
{
    [PrimaryKey] public Guid ID { get; set; }
    public string Name { get; set; }
    public bool IsDefault { get; set; }
    
    [ManyToMany(typeof(UserConversation), nameof(UserConversation.ConversationID), nameof(User.Conversations), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<User> Users { get; set; }

    [OneToMany(nameof(Message.ConversationID), nameof(Message.Conversation), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<Message> Messages { get; set; }

    public Conversation()
    {
        ID = Guid.NewGuid();
        Name = string.Empty;
        Users = new List<User>();
        Messages = new List<Message>();
    }
    
    public Conversation(List<User> users, string name = "Default", bool isDefault = true)
    {
        ID = Guid.NewGuid();
        Name = name;
        Users = users;
        Messages = new List<Message>();
        IsDefault = isDefault && Users.Count <= 2;
    }

    public void AddMessages(IEnumerable<Message> messages)
    {
        Messages = Messages.Union(messages).OrderBy(m => m.DateTime).ToList();
    }
}