using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olympus.Helios.Equipment.Model
{
    public abstract class Machine
    {   
        [PrimaryKey]
        public abstract int SerialNumber { get; set; }
        public abstract string TypeCode { get; set; }
        public abstract DateTime ServiceDueDate { get; set; }
        public abstract DateTime LastServiceDate { get; set; }
        public abstract DateTime LastPreOpCheck { get; set; }
        public abstract string Ownership { get; set; }
        public abstract string LicenceCode { get; set; }
        [ForeignKey(typeof(Checklist))]
        public abstract string ChecklistName { get; set; }

        [ManyToOne]
        public abstract Checklist Checklist { get; set; }

    }
}
