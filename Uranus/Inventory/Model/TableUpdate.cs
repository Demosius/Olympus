using SQLite;
using System;

namespace Uranus.Inventory.Model
{
    [Table("UpdateTimes")]
    public class TableUpdate
    {
        [PrimaryKey]
        public string TableName { get; set; }
        public DateTime LastUpdate { get; set; }

        /* OPERATOR OVERLOADING */
        /*public override bool Equals(object obj) => this.Equals(obj as TableUpdate);

        public bool Equals(TableUpdate tableUpdate)
        {
            if (tableUpdate is null) return false;

            if (ReferenceEquals(this, tableUpdate)) return true;

            if (GetType() != tableUpdate.GetType()) return false;

            return TableName == tableUpdate.TableName && LastUpdate == tableUpdate.LastUpdate;
        }

        public static bool operator ==(TableUpdate lhs, TableUpdate rhs)
        {
            if (lhs is not null) return rhs != null && lhs.LastUpdate == rhs.LastUpdate;
            return rhs is null;
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

        public static bool operator <=(TableUpdate lhs, TableUpdate rhs) => lhs == rhs || lhs < rhs;*/
        
    }
}
