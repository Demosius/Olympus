using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olympus.Helios.Equipment.Model
{
    public class Stockpicker : Machine
    {
        public bool HighReach() => Type.AccessLevel == Inventory.EAccessLevel.HighReach;
    }
}
