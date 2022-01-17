using Uranus.Users.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Users
{
    public class UserCreator
    {
        public UserChariot Chariot { get; set; }

        public UserCreator(ref UserChariot chariot)
        {
            Chariot = chariot;
        }

        public void AssureDefaultRole()
        {
            if (Chariot.Database.Execute("SELECT count(*) FROM Role WHERE Name='Default';") > 0) return;

            Role role = new();

            role.SetDefault();

            _ = Chariot.Create(role);
        }

        public void AssureDBManagerRole()
        {
            if (Chariot.PullObjectList<Role>().Where(Role => Role.Name == "DatabaseManager").Any()) return;

            Role role = new();
            role.SetMaster();
            _ = Chariot.Create(role);
        }

        public void Role(Role role, PushType pushType = PushType.ObjectOnly) => Chariot.Create(role, pushType);

        public void User(User user, PushType pushType = PushType.ObjectOnly) => Chariot.Create(user, pushType);

        public void Login(Login login, PushType pushType = PushType.ObjectOnly) => Chariot.Create(login, pushType);
    }
}
