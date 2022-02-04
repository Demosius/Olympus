using Uranus.Staff;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using Olympus.ViewModel.Components;
using Olympus.ViewModel.Commands;
using System.IO;
using ServiceStack.Text;
using SQLite;
using System.Windows;
using Uranus.Staff.Model;
using Uranus;
using Olympus.ViewModel.Utility;
using Olympus.Properties;
using Uranus.Inventory.Model;

namespace Olympus.ViewModel
{

    public class OlympusVM : INotifyPropertyChanged
    {
        public Dictionary<EProject, IProject> RunningProjects { get; set; }

        private IProject currentProject;
        public IProject CurrentProject
        {
            get => currentProject;
            set
            {
                currentProject = value;
                OnPropertyChanged(nameof(CurrentProject));
            }
        }

        /* Sub ViewModels - Components */
        public DBManager DBManager { get; set; }
        public InventoryUpdaterVM InventoryUpdaterVM { get; set; }
        public ProjectLauncherVM ProjectLauncherVM { get; set; }
        public UserHandlerVM UserHandlerVM { get; set; }

        /* Commands */
        public GenerateMasterSkuListCommand GenerateMasterSkuListCommand { get; set; }

        /* Constructor(s) */
        public OlympusVM()
        {
            RunningProjects = new();

            EstablishInitialProjectIcons();

            DBManager = new(this);
            UserHandlerVM = new(this);
            ProjectLauncherVM = new(this);
            InventoryUpdaterVM = new(this);

            GenerateMasterSkuListCommand = new(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        /* Temporary Functions */

        /// <summary>
        /// Takes all the employees that have any direct reports, and creates a User for each of those employees with the manager role.
        /// </summary>
        public static void AutoGenerateManagers()
        {
            // Get employees.
            var managers = App.Helios.StaffReader.Managers();
            foreach (var m in managers)
                App.Charon.CreateNewUser(m, "Manager");
        }

        internal void RefreshData()
        {
            foreach (var project in RunningProjects)
            {
                project.Value.RefreshData();
            }
        }

        internal void ResetDB()
        {
            App.Helios.ResetChariots(Settings.Default.SolLocation);
            App.Charon.DatabaseReset(Settings.Default.SolLocation);

            EstablishInitialProjectIcons();

            UserHandlerVM.CheckUser();
            ProjectLauncherVM = new(this);
            InventoryUpdaterVM = new(this);

            OnPropertyChanged(nameof(ProjectLauncherVM));
            OnPropertyChanged(nameof(InventoryUpdaterVM));

            CurrentProject = null;
            RunningProjects = new();
        }

        /* Projects */
        public void LoadProject(EProject project)
        {
            if (!RunningProjects.TryGetValue(project, out var loadedProject))
            {
                loadedProject = ProjectFactory.GetProject(project);
                RunningProjects.Add(project, loadedProject);
            }
            SetPage(loadedProject);
        }
        
        private void SetPage(IProject project)
        {
            CurrentProject = project;
            
            var navigationService = ((Page) CurrentProject).NavigationService;
            if (navigationService != null)
                _ = navigationService.Navigate((Page) project);
            
        }
        
        public void ClearRunningProjects()
        {
            CurrentProject = null;
            RunningProjects = new();
        }
        

        private static void EstablishInitialProjectIcons()
        {
            App.Helios.StaffCreator.CopyProjectIconsFromSource(Path.Combine(App.BaseDirectory(), "Resources", "Images", "Icons"));
            List<Project> projects = new()
            {
                new(EProject.Khaos, "chaos.ico", App.Helios.StaffReader,"Handles make-bulk designation and separation. (Genesis)"),
                new(EProject.Pantheon, "pantheon.ico", App.Helios.StaffReader, "Roster management."),
                new(EProject.Prometheus, "prometheus.ico", App.Helios.StaffReader, "Data management."),
                new(EProject.Phoenix, "phoenix.ico", App.Helios.StaffReader, "Pre-work automated stock maintenance. (AutoBurn)"),
                new(EProject.Vulcan, "vulcan.ico", App.Helios.StaffReader, "Replenishment DDR management and work assignment. (RefOrge)"),
                new(EProject.Aion, "Aion.ico", App.Helios.StaffReader, "Employee clock in and shift time management.")
            };

            App.Helios.StaffCreator.EstablishInitialProjects(projects);
            
        }

        public static void GenerateMasterSkuList()
        {
            var masters = App.Helios.InventoryReader.GetMasters();

            // Make sure the target destination exists.
            var dirPath = Path.Combine(App.BaseDirectory(), "SKUMasterExports");

            Directory.CreateDirectory(dirPath);

            var csvTask = Task.Run(() => ExportMasterSkuAsCSV(masters, dirPath));
            var jsonTask = Task.Run(() => ExportMasterSkuAsJSON(masters, dirPath));
            var xmlTask = Task.Run(() => ExportMasterSkuAsXml(masters, dirPath));
            //var sqlTask = Task.Run(() => ExportMasterSkuIntoSqLite(masters, dirPath));
            Task.WaitAll(csvTask, jsonTask, xmlTask);//, sqlTask);

            _ = MessageBox.Show("Files exported.");
        }

        public static void ExportMasterSkuAsCSV(IEnumerable<SkuMaster> masters, string dirPath)
        {
            var csv = CsvSerializer.SerializeToCsv(masters);
            var filePath = Path.Combine(dirPath, "SKUMasterExport.csv");
            File.WriteAllText(filePath, csv);
        }

        public static void ExportMasterSkuAsJSON(IEnumerable<SkuMaster> masters, string dirPath)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(masters);
            var filePath = Path.Combine(dirPath, "SKUMasterExport.json");
            File.WriteAllText(filePath, json);
        }

        public static void ExportMasterSkuIntoSqLite(IEnumerable<SkuMaster> masters, string dirPath)
        {
            var dbPath = Path.Combine(dirPath, "SKUMasterExport.sqlite");
            using SQLiteConnection database = new(dbPath);
            _ = database.CreateTable(typeof(SkuMaster));
            _ = database.DeleteAll<SkuMaster>();
            _ = database.InsertAll(masters);
        }

        public static void ExportMasterSkuAsXml(IEnumerable<SkuMaster> masters, string dirPath)
        {
            var xml = XmlSerializer.SerializeToString(masters);
            var filePath = Path.Combine(dirPath, "SKUMasterExport.xml");
            File.WriteAllText(filePath, xml);
        }
    }
}
