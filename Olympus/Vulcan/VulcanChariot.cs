using Olympus.Properties;
using Uranus;
using Olympus.Vulcan.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Olympus.Vulcan
{
    /// <summary>
    ///  The chariot for Vulcan specific data transactions and persitence.
    /// </summary>
    class VulcanChariot : MasterChariot
    {
        public override string DatabaseName { get; } = "Vulcan.sqlite";

        public override Type[] Tables { get; } = new Type[] {typeof(Operator)};

        /*************************** Constructors ****************************/

        public VulcanChariot()
        {
            // Try first to use the directory based on Settings.Default, if not then use local file.
            try
            {
                BaseDataDirectory = Path.Combine(Settings.Default.SolLocation, "Vulcan");
                InitializeDatabaseConnection();
            }
            catch
            {
                _ = MessageBox.Show("Reverting to local use database.", "Error loading database.", MessageBoxButton.OK, MessageBoxImage.Warning);
                BaseDataDirectory = Path.Combine(App.BaseDirectory(), "Sol", "Vulcan");
                InitializeDatabaseConnection();
            }
        }

        public VulcanChariot(string solLocation)
        {
            // Try first to use the given directory, if not then use local file.
            try
            {
                BaseDataDirectory = Path.Combine(solLocation, "Vulcan");
                InitializeDatabaseConnection();
            }
            catch
            {
                _ = MessageBox.Show("Reverting to local use database.", "Error loading database.", MessageBoxButton.OK, MessageBoxImage.Warning);
                BaseDataDirectory = Path.Combine(App.BaseDirectory(), "Sol", "Vulcan");
                InitializeDatabaseConnection();
            }
        }

        public override void ResetConnection()
        {
            // Try first to use the directory based on Settings.Default, if not then use local file.
            try
            {
                BaseDataDirectory = Path.Combine(Settings.Default.SolLocation, "Vulcan");
                InitializeDatabaseConnection();
            }
            catch
            {
                _ = MessageBox.Show("Reverting to local use database.", "Error loading database.", MessageBoxButton.OK, MessageBoxImage.Warning);
                BaseDataDirectory = Path.Combine(App.BaseDirectory(), "Sol", "Vulcan");
                InitializeDatabaseConnection();
            }
        }

        /***************************** CREATE Data ****************************/

        /***************************** READ Data ******************************/

        /***************************** UPDATE Data ****************************/

        /***************************** DELETE Data ****************************/
        
    }
}
