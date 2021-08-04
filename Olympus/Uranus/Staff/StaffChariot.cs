using Olympus.Uranus.Staff.Model;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Olympus.Uranus.Staff
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
            typeof(Clan),               typeof(Department),                 typeof(DepartmentProject),  typeof(Employee),                   
            typeof(EmployeeAvatar),     typeof(EmployeeDepartmentLoaning),  typeof(EmployeeIcon),       typeof(EmployeeInductionReference),    
            typeof(EmployeeProject),    typeof(EmployeeShift),              typeof(EmployeeVehicle),    typeof(Induction),                  
            typeof(Licence),            typeof(LicenceImage),               typeof(Locker),             typeof(Project),            
            typeof(ProjectIcon),        typeof(Role),                       typeof(Shift),              typeof(ShiftRule),          
            typeof(TagUse),             typeof(TempTag),                    typeof(Vehicle)
        };

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
            // First thing is to nullify the current databse (connection).
            Database = null;
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

        public void CreateIconDirectories()
        {
            EmployeeIconDirectory = Path.Combine(BaseDataDirectory, "EmployeeIcons");
            EmployeeAvatarDirectory = Path.Combine(BaseDataDirectory, "EmployeeAvatars");
            ProjectIconDirectory = Path.Combine(BaseDataDirectory, "ProjectIcons");
            LicenceImageDirectory = Path.Combine(BaseDataDirectory, "LicenceImages");
            if (!Directory.Exists(BaseDataDirectory)) Directory.CreateDirectory(BaseDataDirectory);
            if (!Directory.Exists(EmployeeIconDirectory)) Directory.CreateDirectory(EmployeeIconDirectory);
            if (!Directory.Exists(EmployeeAvatarDirectory)) Directory.CreateDirectory(EmployeeAvatarDirectory);
            if (!Directory.Exists(ProjectIconDirectory)) Directory.CreateDirectory(ProjectIconDirectory);
            if (!Directory.Exists(LicenceImageDirectory)) Directory.CreateDirectory(LicenceImageDirectory);
            EstablishInitialProjectIcons();
        }

        private void EstablishInitialProjectIcons()
        {
            List<Project> projects = new List<Project> 
            {
                new Project(EProject.Khaos, "chaos.ico", "Handles makebulk designation and separation. (Genesis)"),
                new Project(EProject.Pantheon, "pantheon.ico", "Roster management."),
                new Project(EProject.Prometheus, "prometheus.ico", "Data management."),
                new Project(EProject.Torch, "torch.ico", "Pre-work automated stock maintenance. (AutoBurn)"),
                new Project(EProject.Vulcan, "vulcan.ico", "Replenishment DDR management and work assignment. (RefOrge)")
            };
            List<string> existingProjects = PullObjectList<Project>().Select(p => p.Name).ToList();
            Database.RunInTransaction(() =>
            {
                foreach (Project project in projects)
                {
                    if (!existingProjects.Contains(project.Name)) 
                        Database.InsertWithChildren(project);
                }
            });
            FillProjectIconFolder();
        }

        private void FillProjectIconFolder()
        {
            string resourcePath = Path.Combine(App.BaseDirectory(), "Resources", "Images", "Icons");
            string fileName;
            foreach (string filePath in Directory.GetFiles(resourcePath))
            {
                fileName = Path.GetFileName(filePath);
                if (Path.GetExtension(filePath) == ".ico")
                {
                    File.Copy(filePath, Path.Combine(ProjectIconDirectory, fileName), true);
                }
            }
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
