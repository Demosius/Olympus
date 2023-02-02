using System.Collections.Generic;
using System.Linq;
using SQLite;
using SQLiteNetExtensions.Attributes;
using Uranus.Staff.Models;

namespace Uranus.Users.Models;

public class User
{
    [PrimaryKey] public int ID { get; set; }
    [ForeignKey(typeof(Role))] public string RoleName { get; set; }
    public bool IsDev { get; set; }

    [ManyToOne(nameof(RoleName), nameof(Models.Role.Users), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Role? Role { get; set; }

    [ManyToMany(typeof(UserConversation), nameof(UserConversation.UserID), nameof(Conversation.Users), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<Conversation> Conversations { get; set; }

    [OneToMany(nameof(Message.UserID), nameof(Message.User), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<Message> Messages { get; set; }

    [Ignore]
    public Employee? Employee { get; set; }

    public User()
    {
        RoleName = string.Empty;
        Conversations = new List<Conversation>();
        Messages = new List<Message>();
    }

    public User(int id, string roleName) : this()
    {
        ID = id;
        RoleName = roleName;
    }

    public User(Employee employee, Role role) : this(employee.ID, role.Name)
    {
        Employee = employee;
        Role = role;
    }

    public void SetRole(Role role)
    {
        Role = role;
        RoleName = role.Name;
    }

    public void AddMessage(Message message)
    {
        Messages.Add(message);
        message.User = this;
    }

    public void AddMessages(IEnumerable<Message> messages)
    {
        var msgList = messages.ToList();

        foreach (var message in msgList) message.User = this;

        Messages = Messages.Union(msgList).OrderBy(m => m.ConversationID).ThenBy(m => m.DateTime).ToList();
    }

    public void AddConversation(Conversation conversation)
    {
        Conversations.Add(conversation);
        conversation.Users.Add(this);
    }
}