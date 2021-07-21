using Olympus.Helios.Staff.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Olympus.Helios.Staff
{
    public class StaffChariot : MasterChariot
    {
        public override string DatabaseName { get; } = "Staff.sqlite";

        public override Type[] Tables { get; } = new Type[] {typeof(Clan), typeof(Department), typeof(Employee), typeof(EmployeeDepartmentLoaning),
                                                             typeof(EmployeeInductionReference), typeof(EmployeeVehicle), typeof(EmployeeShift),
                                                             typeof(Induction), typeof(Licence), typeof(Locker),
                                                             typeof(Licence), typeof(Locker), typeof(Role),
                                                             typeof(Role), typeof(Shift), typeof(ShiftRule),
                                                             typeof(TagUse), typeof(TempTag), typeof(Vehicle)};

        /*************************** Constructors ****************************/

        public StaffChariot()
        {
            // Try first to use the directory based on App.Settings, if not then use local file.
            try
            {
                BaseDataDirectory = Path.Combine(App.Settings.SolLocation, "Staff");
                InitializeDatabaseConnection();
            }
            catch
            {
                MessageBox.Show("Reverting to local use database.", "Error loading database.", MessageBoxButton.OK, MessageBoxImage.Warning);
                BaseDataDirectory = Path.Combine(App.BaseDirectory(), "Sol", "Staff");
                InitializeDatabaseConnection();
            }
        }

        public StaffChariot(string solLocation)
        {
            // Try first to use the given directory, if not then use local file.
            try
            {
                BaseDataDirectory = Path.Combine(solLocation, "Staff");
                InitializeDatabaseConnection();
            }
            catch
            {
                MessageBox.Show("Reverting to local use database.", "Error loading database.", MessageBoxButton.OK, MessageBoxImage.Warning);
                BaseDataDirectory = Path.Combine(App.BaseDirectory(), "Sol", "Staff");
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
                BaseDataDirectory = Path.Combine(App.BaseDirectory(), "Sol", "Staff");
                InitializeDatabaseConnection();
            }
        }

        /***************************** CREATE Data ****************************/

        /****************************** READ Data *****************************/

        /***************************** UPDATE Data ****************************/

        /***************************** DELETE Data ****************************/

    }
}
