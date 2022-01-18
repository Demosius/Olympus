using Uranus.Staff.Model;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Uranus.Staff
{
    public class StaffChariot : MasterChariot
    {
        public override string DatabaseName { get; } = "Staff.sqlite";
        public string EmployeeIconDirectory { get; set; }
        public string EmployeeAvatarDirectory { get; set; }
        public string ProjectIconDirectory { get; set; }
        public string LicenceImageDirectory { get; set; }
        
        public override Type[] Tables { get; } = new Type[]
        {
            typeof(Clan),                       typeof(ClockEvent),         typeof(Department),                 typeof(DepartmentProject),  
            typeof(Employee),                   typeof(EmployeeAvatar),     typeof(EmployeeDepartmentLoaning),  typeof(EmployeeIcon),       
            typeof(EmployeeInductionReference), typeof(EmployeeProject),    typeof(EmployeeShift),              typeof(EmployeeVehicle),    
            typeof(Induction),                  typeof(Licence),            typeof(LicenceImage),               typeof(Locker),            
            typeof(Project),                    typeof(ProjectIcon),        typeof(Role),                       typeof(Shift),              
            typeof(ShiftRule),                  typeof(TagUse),             typeof(TempTag),                    typeof(Vehicle)
        };

        /*************************** Constructors ****************************/

        public StaffChariot(string solLocation)
        {
            // Try first to use the given directory, if not then use local file.
            try
            {
                BaseDataDirectory = Path.Combine(solLocation, "Staff");
                InitializeDatabaseConnection();
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Resets the connection using the given location string.
        /// </summary>
        /// <param name="solLocation">A directory location, in which the Staff database does/should reside.</param>
        public void ResetConnection(string solLocation)
        {
            // First thing is to nullify the current databse (connection).
            Database = null;

            try
            {
                BaseDataDirectory = Path.Combine(solLocation, "Staff");
                InitializeDatabaseConnection();
            }
            catch (Exception) { throw; }
        }

        public void CreateIconDirectories()
        {
            EmployeeIconDirectory = Path.Combine(BaseDataDirectory, "EmployeeIcons");
            EmployeeAvatarDirectory = Path.Combine(BaseDataDirectory, "EmployeeAvatars");
            ProjectIconDirectory = Path.Combine(BaseDataDirectory, "ProjectIcons");
            LicenceImageDirectory = Path.Combine(BaseDataDirectory, "LicenceImages");
            if (!Directory.Exists(BaseDataDirectory)) _ = Directory.CreateDirectory(BaseDataDirectory);
            if (!Directory.Exists(EmployeeIconDirectory)) _ = Directory.CreateDirectory(EmployeeIconDirectory);
            if (!Directory.Exists(EmployeeAvatarDirectory)) _ = Directory.CreateDirectory(EmployeeAvatarDirectory);
            if (!Directory.Exists(ProjectIconDirectory)) _ = Directory.CreateDirectory(ProjectIconDirectory);
            if (!Directory.Exists(LicenceImageDirectory)) _ = Directory.CreateDirectory(LicenceImageDirectory);
        }

        protected override void InitializeDatabaseConnection()
        {
            base.InitializeDatabaseConnection();
            CreateIconDirectories();
        }

        /***************************** CREATE Data ****************************/

        /****************************** READ Data *****************************/

        /***************************** UPDATE Data ****************************/

        /***************************** DELETE Data ****************************/

    }
}
