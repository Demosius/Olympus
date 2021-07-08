using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Olympus.Helios.Staff.Model;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Olympus.Helios.Users.Model
{
    public class User
    {
        [PrimaryKey]
        public int ID { get; set; }
        [ForeignKey(typeof(Role))]
        public string RoleName { get; set; }

        [ManyToOne]
        public Role Role { get; set; }

        [Ignore]
        public Employee Employee { get; set; }

    }

    
}
