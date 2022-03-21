using Uranus.Users.Model;

namespace Uranus.Users;

public class UserReader
{
    public UserChariot Chariot { get; set; }

    public UserReader(ref UserChariot chariot)
    {
        Chariot = chariot;
    }

    public bool UserExists(int userID) => Chariot.Database?.ExecuteScalar<int>("SELECT count(*) FROM User WHERE ID=?;", userID) > 0;

    public Login? Login(int userID) => Chariot.PullObject<Login>(userID);

    public User? User(int userID) => Chariot.PullObject<User>(userID, EPullType.FullRecursive);

    public Role? Role(string roleName) => Chariot.PullObject<Role>(roleName, EPullType.FullRecursive);

    public int UserCount() => Chariot.PullObjectList<User>(pullType: EPullType.ObjectOnly).Count; //Chariot.Database.Execute("SELECT count(*) FROM User;");

}