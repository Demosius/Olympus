using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olympus.Helios.Equipment
{
    public class Forklift : Machine
    {
        public bool HighReach()
        {
            return TypeCode.Contains("HR");
        }
    }
}
