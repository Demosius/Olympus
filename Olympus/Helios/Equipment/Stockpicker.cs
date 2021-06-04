using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olympus.Helios.Equipment
{
    public class Stockpicker : Machine
    {
        public Stockpicker(int serialNo, string typeCode, DateTime serviceDueDate, DateTime lastServiceDate, Checklist checklist, string ownership, string licenceCode, DateTime lastPreop) 
            : base(serialNo, typeCode, serviceDueDate, lastServiceDate, checklist, ownership, licenceCode, lastPreop)
        {
        }

        public bool HighReach()
        {
            return TypeCode.Contains("HR");
        }
    }
}
