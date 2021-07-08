using SQLite;
using SQLiteNetExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory.Model
{
    [Table("UpdateTimes")]
    class TableUpdate
    {
        [PrimaryKey]
        public string TableName { get; set; }
        public DateTime LastUpdate { get; set; }

        /* OPERATOR OVERLOADING */
        public override bool Equals(object obj) => this.Equals(obj as TableUpdate);

        public bool Equals(TableUpdate tableUpdate)
        {
            if (tableUpdate is null) return false;

            if (Object.ReferenceEquals(this, tableUpdate)) return true;

            if (this.GetType() != tableUpdate.GetType()) return false;

            return TableName == tableUpdate.TableName && LastUpdate == tableUpdate.LastUpdate;
        }

        public override int GetHashCode() => (TableName, LastUpdate).GetHashCode();

        public static bool operator ==(TableUpdate lhs, TableUpdate rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }
                return false;
            }
            return lhs.LastUpdate == rhs.LastUpdate;
        }

        public static bool operator !=(TableUpdate lhs, TableUpdate rhs) => !(lhs == rhs);

        public static bool operator >(TableUpdate lhs, TableUpdate rhs)
        {
            if (lhs is null || rhs is null) return false;
            return lhs.LastUpdate > rhs.LastUpdate;
        }

        public static bool operator <(TableUpdate lhs, TableUpdate rhs)
        {
            if (lhs is null || rhs is null) return false;
            return lhs.LastUpdate < rhs.LastUpdate;
        }

        public static bool operator >=(TableUpdate lhs, TableUpdate rhs) => lhs == rhs || lhs > rhs;

        public static bool operator <=(TableUpdate lhs, TableUpdate rhs) => lhs == rhs || lhs < rhs;
        
    }
}
