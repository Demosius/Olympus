using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olympus.Helios.Equipment.Model
{
    public class Rabbit : Machine
    {
        [PrimaryKey]
        public override int SerialNumber { get; set; }
        public override string TypeCode { get; set; }
        public override DateTime ServiceDueDate { get; set; }
        public override DateTime LastServiceDate { get; set; }
        public override DateTime LastPreOpCheck { get; set; }
        public override string Ownership { get; set; }
        public override string LicenceCode { get; set; }
        [ForeignKey(typeof(Checklist))]
        public override string ChecklistName { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public override Checklist Checklist { get; set; }

    }
}
