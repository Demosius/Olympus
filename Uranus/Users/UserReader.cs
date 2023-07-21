using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Uranus.Users.Models;

namespace Uranus.Users;

public class UserReader
{
    public UserChariot Chariot { get; set; }

    public UserReader(ref UserChariot chariot)
    {
        Chariot = chariot;
    }

    public bool UserExists(int userID) => Chariot.ExecuteScalar<int>("SELECT count(*) FROM User WHERE ID=?;", userID) > 0;

    public Login? Login(int userID) => Chariot.PullObject<Login>(userID);

    public User? User(int userID) => Chariot.PullObject<User>(userID, EPullType.FullRecursive);

    public Role? Role(string roleName) => Chariot.PullObject<Role>(roleName, EPullType.FullRecursive);

    public int UserCount() => Chariot.ExecuteScalar<int>("SELECT count(*) FROM User;"); /*Chariot.PullObjectListAsync<User>(pullType: EPullType.ObjectOnly).Count;*/

    public async Task<IEnumerable<User>> UsersAsync(Expression<Func<User, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public async Task<IEnumerable<Role>> RolesAsync(Expression<Func<Role, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);


    /* Messages */
    public Message? Message(Guid id, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<Message>(id, pullType);

    public IEnumerable<Message> Messages(Expression<Func<Message, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public IEnumerable<Message> Messages(Guid conversationID) => Messages(m => m.ConversationID == conversationID);

    public IEnumerable<Message> Messages(Conversation conversation) => Messages(conversation.ID);

    public Conversation? DefaultConversation(User user, User targetUser)
    {
        Conversation? returnVal = null;
        Chariot.Database?.RunInTransaction(() =>
        {
            var ucList = UserConversations(uc => uc.UserID == user.ID || uc.UserID == targetUser.ID);
            var convoIDList = ucList.Select(uc => uc.ConversationID).Distinct();
            var convos = Conversations(conversation => convoIDList.Contains(conversation.ID) && conversation.IsDefault).ToList();
            if (convos.Any())
            {
                returnVal = convos.First();
            }
        });
        return returnVal;
    }

    public IEnumerable<Conversation> Conversations(Expression<Func<Conversation, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public IEnumerable<UserConversation> UserConversations(Expression<Func<UserConversation, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public HermesDataSet HermesDataSet(User user)
    {
        HermesDataSet? hermesDataSet = null;
        Chariot.Database?.RunInTransaction(() =>
        {
            var userConvos = UserConversations(uc => uc.UserID == user.ID);
            var convoIDs = userConvos.Select(uc => uc.ConversationID);
            var conversations = Conversations(c => convoIDs.Contains(c.ID));
            var messages = Messages(m => convoIDs.Contains(m.ConversationID));
            userConvos = UserConversations(uc => convoIDs.Contains(uc.ConversationID)).ToList();
            var userIDs = userConvos.Select(uc => uc.UserID);
            var contacts = Users(u => userIDs.Contains(u.ID));

            hermesDataSet = new HermesDataSet(user, contacts, conversations, messages, userConvos);
        });
        return hermesDataSet ??= new HermesDataSet(user);
    }
}