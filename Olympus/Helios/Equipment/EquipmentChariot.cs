using Olympus.Helios.Equipment.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Olympus.Helios.Equipment
{
    public class EquipmentChariot : MasterChariot
    {
        public override string DatabaseName { get; } = "Equipment.sqlite";

        public override Type[] Tables { get; } = new Type[] {typeof(Checklist), typeof(Forklift),
                                                             typeof(Rabbit), typeof(Stockpicker)};

        /*************************** Constructors ****************************/

        public EquipmentChariot()
        {
            // Try first to use the directory based on App.Settings, if not then use local file.
            try
            {
                BaseDataDirectory = Path.Combine(App.Settings.SolLocation, "Equipment");
                InitializeDatabaseConnection();
            }
            catch
            {
                MessageBox.Show("Reverting to local use database.", "Error loading database.", MessageBoxButton.OK, MessageBoxImage.Warning);
                BaseDataDirectory = Path.Combine(App.BaseDirectory(), "Sol", "Equipment");
                InitializeDatabaseConnection();
            }
        }

        public EquipmentChariot(string solLocation)
        {
            // Try first to use the given directory, if not then use local file.
            try
            {
                BaseDataDirectory = Path.Combine(solLocation, "Equipment");
                InitializeDatabaseConnection();
            }
            catch
            {
                MessageBox.Show("Reverting to local use database.", "Error loading database.", MessageBoxButton.OK, MessageBoxImage.Warning);
                BaseDataDirectory = Path.Combine(App.BaseDirectory(), "Sol", "Equipment");
                InitializeDatabaseConnection();
            }
        }

        public override void ResetConnection()
        {
            // Try first to use the directory based on App.Settings, if not then use local file.
            try
            {
                BaseDataDirectory = Path.Combine(App.Settings.SolLocation, DatabaseName);
                InitializeDatabaseConnection();
            }
            catch
            {
                MessageBox.Show("Reverting to local use database.", "Error loading database.", MessageBoxButton.OK, MessageBoxImage.Warning);
                BaseDataDirectory = Path.Combine(App.BaseDirectory(), "Sol", "Equipment");
                InitializeDatabaseConnection();
            }
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
