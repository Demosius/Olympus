using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uranus.Equipment.Model
{
    public class Forklift : Machine
    {
        public bool HighReach() => Type.AccessLevel == Inventory.EAccessLevel.HighReach;
    }
}
