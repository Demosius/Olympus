using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olympus.Helios.Inventory
{   
    /// <summary>
    ///  A class representing the zones within the warehouse.
    /// </summary>
    public class Zone
    {
        public string Code { get; }
        public string Type { get; set; }
        public string Description { get; set; }

        public Dictionary<string, Bay> Bays { get; }

        /// <summary>
        ///  Create a zone based primarily on the Code name.
        /// </summary>
        /// <param name="code">The name/code of the zone.</param>
        /// <param name="type">Primary/Overstock/etc.</param>
        /// <param name="description">Other notes such as high reach/ground/etc.</param>
        public Zone(string code, string type, string description)
        {
            Code = code;
            Type = type;
            Description = description;

            Bays = new Dictionary<string, Bay> { };
        }

        /// <summary>
        ///  Add an existing bay to the zone if one of the same name doesn't already exist.
        /// </summary>
        /// <param name="bay"></param>
        public void AddBay(Bay bay)
        {
            if (!Bays.ContainsKey(bay.Name))
            {
                Bays.Add(bay.Name, bay);
            }
        }

        /// <summary>
        /// Add a bay to the zone (if it doesn't exist) by creating a new bay from the string name given.
        /// </summary>
        /// <param name="bayName"></param>
        public void AddBay(string bayName)
        {
            if(!Bays.ContainsKey(bayName))
            {
                Bay bay = new Bay(this, bayName);
                Bays.Add(bay.Name, bay);
            }
        }

        /// <summary>
        ///  Add a bin to the zone, placing the bin within a bay in the zone, creating that bay if necessary.
        /// </summary>
        /// <param name="bin"></param>
        public void AddBin(Bin bin)
        {
            string bayName = bin.GetBayName();
            if (!Bays.ContainsKey(bayName)) AddBay(bayName);
            Bays[bayName].AddBin(bin);
        }
    }
}
