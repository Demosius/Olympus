using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uranus.Staff.Model;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Users.Model
{
    public class User
    {
        [PrimaryKey]
        public int ID { get; set; }
        [ForeignKey(typeof(Role))]
        public string RoleName { get; set; }

        private Role role;
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Role Role { get => role; set { role = value; RoleName = value.Name; } }

        [Ignore]
        public Employee Employee { get; set; }

    }

    
}
