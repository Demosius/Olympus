﻿using Olympus.Helios.Staff.Model;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff
{
    public class StaffReader
    {
        public StaffChariot Chariot { get; set; }

        public StaffReader(ref StaffChariot chariot)
        {
            Chariot = chariot;
        }

    }
}