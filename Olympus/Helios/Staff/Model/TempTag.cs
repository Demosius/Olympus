using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff.Model
{
    public class TempTag
    {
        [PrimaryKey]
        public string RF_ID { get; set; }

        [OneToMany]
        public TagUse TagUse { get; set; }
    }
}
