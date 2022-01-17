using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Users.Model
{
    public class Login
    {
        [PrimaryKey, ForeignKey(typeof(User))]
        public int UserID { get; set; }
        public string Salt { get; set; }
        public string PasswordHash { get; set; }

    }
}
