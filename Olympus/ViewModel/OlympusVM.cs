using System;
using Phoenix.View;
using Uranus.Staff;
using Pantheon.View;
using Prometheus.View;
using Vulcan.View;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Khaos.View;
using Olympus.ViewModel.Components;
using Olympus.Model;
using Olympus.ViewModel.Commands;
using System.IO;
using ServiceStack.Text;
using SQLite;
using System.Windows;
using Uranus.Staff.Model;
using Uranus;
using Aion.View;
using Olympus.ViewModel.Utility;
using Olympus.Properties;

namespace Olympus.ViewModel
{

    public class OlympusVM : INotifyPropertyChanged
    {
        public PrometheusPage Prometheus { get; set; }
        public PantheonPage Pantheon { get; set; }
        public VulcanPage Vulcan { get; set; }
        public PhoenixPage Phoenix { get; set; }
        public KhaosPage Khaos { get; set; }
        public AionPage Aion { get; set; }

        public EProject CurrentProject { get; set; }

        private Page currentPage;
        public Page CurrentPage
        {
            get => currentPage;
            set
            {
                currentPage = value;
                if (value is not null) CurrentProject = ((IProject) value).Project;
                OnPropertyChanged(nameof(CurrentPage));
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
            Prometheus?.RefreshData();
            Pantheon?.RefreshData();
            Vulcan?.RefreshData();
            Aion?.RefreshData();
            Phoenix?.RefreshData();
            Khaos?.RefreshData();
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

            Prometheus = null;
            Pantheon = null;
            Vulcan = null;
            Aion = null;
            Phoenix = null;
            Khaos = null;
        }

        /* Projects */
        public void LoadProject(EProject project)
        {
            switch (project)
            {
                case EProject.Vulcan:
                    LoadVulcan();
                    break;
                case EProject.Prometheus:
                    LoadPrometheus();
                    break;
                case EProject.Pantheon:
                    LoadPantheon();
                    break;
                case EProject.Phoenix:
                    LoadTorch();
                    break;
                case EProject.Khaos:
                    LoadKhaos();
                    break;
                case EProject.Aion:
                    LoadAion();
                    break;
                case EProject.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(project), project, null);
            }
        }

        private void LoadAion()
        {
            Aion ??= new();
            SetPage(Aion);
        }

        private void LoadPrometheus()
        {
            Prometheus ??= new();
            SetPage(Prometheus);
        }

        private void LoadPantheon()
        {
            Pantheon ??= new();
            SetPage(Pantheon);
        }

        private void LoadVulcan()
        {
            Vulcan ??= new();
            SetPage(Vulcan);
        }

        private void LoadTorch()
        {
            Phoenix ??= new();
            SetPage(Phoenix);
        }

        private void LoadKhaos()
        {
            Khaos ??= new();
            SetPage(Khaos);
        }

        private void SetPage(IProject project)
        {
            var page = project as Page;
            if (CurrentPage is null)
                CurrentPage = page;
            else if (CurrentPage.NavigationService != null) 
                _ = CurrentPage.NavigationService.Navigate(page);
        }

        public static List<SkuMaster> GetMasters()
        {
            // TODO: Change to run all DB actions in a single transaction.
            // Set tasks to pull data from db.
            var getItemsTask = Task.Run(() => App.Helios.InventoryReader.NAVItems());
            var getStockTask = Task.Run(() => App.Helios.InventoryReader.NAVAllStock()
                .GroupBy(s => s.ItemNumber)
                .ToDictionary(g => g.Key, g => g.ToList()));
            var getUoMTask = Task.Run(() => App.Helios.InventoryReader.NAVUoMs()
                .GroupBy(u => u.ItemNumber)
                .ToDictionary(g => g.Key, g => g.ToDictionary(u => u.Code, u => u)));
            var getBinsTask = Task.Run(() => App.Helios.InventoryReader.NAVBins().ToDictionary(b => b.ID, b => b));
            var getDivsTask = Task.Run(() => App.Helios.InventoryReader.NAVDivisions().ToDictionary(d => d.Code, d => d.Description));
            var getCatsTask = Task.Run(() => App.Helios.InventoryReader.NAVCategories().ToDictionary(c => c.Code, c => c.Description));
            var getPFsTask = Task.Run(() => App.Helios.InventoryReader.NAVPlatforms().ToDictionary(p => p.Code, p => p.Description));
            var getGensTask = Task.Run(() => App.Helios.InventoryReader.NAVGenres().ToDictionary(g => g.Code, g => g.Description));
            // Wait for tasks to complete.
            Task.WaitAll(getBinsTask, getCatsTask, getDivsTask, getGensTask, getItemsTask, getPFsTask, getStockTask, getUoMTask);
            // Assign results to data lists/dict.
            var navItems = getItemsTask.Result;
            var stock = getStockTask.Result;
            var uomDict = getUoMTask.Result;
            var bins = getBinsTask.Result;
            var divisions = getDivsTask.Result;
            var categories = getCatsTask.Result;
            var platforms = getPFsTask.Result;
            var genres = getGensTask.Result;
            // Generate Sku Master List
            return navItems.Select(item => new SkuMaster(item, stock, uomDict, bins, divisions, categories, platforms, genres)).ToList();
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
            var masters = GetMasters();

            // Make sure the target destination exists.
            var dirPath = Path.Combine(App.BaseDirectory(), "SKUMasterExports");

            Directory.CreateDirectory(dirPath);

            var csvTask = Task.Run(() => ExportMasterSkuAsCSV(masters, dirPath));
            var jsonTask = Task.Run(() => ExportMasterSkuAsJSON(masters, dirPath));
            var xmlTask = Task.Run(() => ExportMasterSkuAsXml(masters, dirPath));
            var sqlTask = Task.Run(() => ExportMasterSkuIntoSqLite(masters, dirPath));
            Task.WaitAll(csvTask, jsonTask, xmlTask, sqlTask);

            _ = MessageBox.Show("Files exported.");
        }

        public static void ExportMasterSkuAsCSV(List<SkuMaster> masters, string dirPath)
        {
            var csv = CsvSerializer.SerializeToCsv(masters);
            var filePath = Path.Combine(dirPath, "SKUMasterExport.csv");
            File.WriteAllText(filePath, csv);
        }

        public static void ExportMasterSkuAsJSON(List<SkuMaster> masters, string dirPath)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(masters);
            var filePath = Path.Combine(dirPath, "SKUMasterExport.json");
            File.WriteAllText(filePath, json);
        }

        public static void ExportMasterSkuIntoSqLite(List<SkuMaster> masters, string dirPath)
        {
            var dbPath = Path.Combine(dirPath, "SKUMasterExport.sqlite");
            using SQLiteConnection database = new(dbPath);
            _ = database.CreateTable(typeof(SkuMaster));
            _ = database.DeleteAll<SkuMaster>();
            _ = database.InsertAll(masters);
        }

        public static void ExportMasterSkuAsXml(List<SkuMaster> masters, string dirPath)
        {
            var xml = XmlSerializer.SerializeToString(masters);
            var filePath = Path.Combine(dirPath, "SKUMasterExport.xml");
            File.WriteAllText(filePath, xml);
        }
    }
}
