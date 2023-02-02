using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Users.Models;

public class HermesDataSet
{
    public User User { get; set; }
    public Dictionary<int, User> Contacts { get; set; }
    public Dictionary<int, Conversation> DefaultConversations { get; set; }
    public Dictionary<Guid, Conversation> Conversations { get; set; }
    public Dictionary<Guid, List<Message>> ConversationMessages { get; set; }
    public Dictionary<int, List<Message>> UserMessages { get; set; }

    public HermesDataSet(User user)
    {
        User = user;
        Contacts = new Dictionary<int, User>();
        DefaultConversations = new Dictionary<int, Conversation>();
        Conversations = new Dictionary<Guid, Conversation>();
        ConversationMessages = new Dictionary<Guid, List<Message>>();
        UserMessages = new Dictionary<int, List<Message>>();
    }

    public HermesDataSet(User user, IEnumerable<User> users, IEnumerable<Conversation> conversations,
        IEnumerable<Message> messages, IEnumerable<UserConversation> userConvoLinks)
    {
        User = user;

        var convos = conversations.ToList();
        var messageList = messages.ToList();
        
        Conversations = convos.ToDictionary(c => c.ID, c => c);
        Contacts = users.ToDictionary(c => c.ID, c => c);
        ConversationMessages = messageList.GroupBy(m => m.ConversationID).ToDictionary(g => g.Key, g => g.ToList());
        UserMessages= messageList.GroupBy(m => m.UserID).ToDictionary(g => g.Key, g => g.ToList());
        
        var ucDict = userConvoLinks.Where(uc => uc.UserID != User.ID).GroupBy(uc => uc.ConversationID)
            .ToDictionary(g => g.Key, g => g.ToList());
        
        DefaultConversations = convos.Where(c => c.IsDefault && ucDict.ContainsKey(c.ID)).ToDictionary(c => ucDict[c.ID].FirstOrDefault()?.UserID ?? 0, c => c);

        Contacts.Add(User.ID, User);

        SetFromMessages();
        SetFromConversations(ucDict);

        Contacts.Remove(User.ID);
    }

    private void SetFromMessages()
    {
        foreach (var (convoID, messageList) in ConversationMessages)
        {
            if (Conversations.TryGetValue(convoID, out var conversation))
            {
                conversation.AddMessages(messageList);
            }
        }

        foreach (var (userID, userMessages) in UserMessages)
        {
            if (Contacts.TryGetValue(userID, out var user))
            {
                user.AddMessages(userMessages);
            }
        }
    }

    private void SetFromConversations(IReadOnlyDictionary<Guid, List<UserConversation>> userConvoLinks)
    {
        foreach (var (convoID, conversation) in Conversations)
        {
            if (!userConvoLinks.TryGetValue(convoID, out var ucList)) continue;
            
            foreach (var uc in ucList)
            {
                if (Contacts.TryGetValue(uc.UserID, out var user))
                {
                    user.AddConversation(conversation);
                }
            }
        }
    }
}