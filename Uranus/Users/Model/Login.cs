using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Users.Model;

public class Login
{
    [PrimaryKey, ForeignKey(typeof(User))] public int UserID { get; set; }
    public string Salt { get; set; }
    public string PasswordHash { get; set; }

}