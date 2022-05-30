using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Users.Models;

public class Login
{
    [PrimaryKey, ForeignKey(typeof(User))] public int UserID { get; set; }
    public string Salt { get; set; }
    public string PasswordHash { get; set; }

    public Login()
    {
        Salt = string.Empty;
        PasswordHash = string.Empty;
    }

    public Login(int userID, string salt, string passwordHash)
    {
        UserID = userID;
        Salt = salt;
        PasswordHash = passwordHash;
    }
}