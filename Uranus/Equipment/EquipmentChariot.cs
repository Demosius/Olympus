using Uranus.Equipment.Model;
using System;
using System.IO;

namespace Uranus.Equipment
{
    public class EquipmentChariot : MasterChariot
    {
        public override string DatabaseName { get; } = "Equipment.sqlite";

        public override Type[] Tables { get; } = new Type[] 
        {
            typeof(Checklist), typeof(CompletedChecklist), typeof(MachineType), typeof(Machine)
        };

        /*************************** Constructors ****************************/

        public EquipmentChariot(string solLocation)
        {
            // Try first to use the given directory, if not then use local file.
            try
            {
                BaseDataDirectory = Path.Combine(solLocation, "Equipment");
                InitializeDatabaseConnection();
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Resets the connection using the given location string.
        /// </summary>
        /// <param name="solLocation">A directory location, in which the Equipment database does/should reside.</param>
        public void ResetConnection(string solLocation)
        {
            // First thing is to nullify the current databse (connection).
            Database = null;

            try
            {
                BaseDataDirectory = Path.Combine(solLocation, "Equipment");
                InitializeDatabaseConnection();
            }
            catch (Exception) { throw; }
        }

        /***************************** CREATE Data ****************************/

        /******************************* Get Data *****************************/
        /* Batteries */

        /* Chargers */

        /* Checklists */

        /* Machine */

        /******************************* Put Data *****************************/

        /****************************** Post Data *****************************/

        /***************************** Delete Data ****************************/


    }
}
