﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olympus.Helios.Equipment
{

    public class Machine
    {
        public int SerialNumber { get; set; }
        public string TypeCode { get; set; }
        public DateTime ServiceDueDate { get; set; }
        public DateTime LastServiceDate { get; set; }
        public DateTime LastPreOpCheck { get; set; }
        public Checklist Checklist { get; set; }
        public string Ownership { get; set; }
        public string LicenceCode { get; set; }

        public Machine() { }

        public Machine(int serialNo, string typeCode, DateTime serviceDueDate, DateTime lastServiceDate, Checklist checklist, string ownership, string licenceCode, DateTime lastPreop)
        {
            SerialNumber = serialNo;
            TypeCode = typeCode;
            ServiceDueDate = serviceDueDate;
            LastServiceDate = lastServiceDate;
            Checklist = checklist;
            Ownership = ownership;
            LicenceCode = licenceCode;
            LastPreOpCheck = lastPreop;
        }
    }
}
