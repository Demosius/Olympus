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
using Uranus.Inventory.Model;
using System.IO;
using ServiceStack.Text;
using SQLite;
using System.Windows;
using Uranus.Staff.Model;
using Uranus;
using System;
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
                if (!(value is null)) CurrentProject = (value as IProject).EProject;
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /* Temporary Functions */

        /// <summary>
        /// Takes all the employees that have any direct reports, and creates a User for each of those employees with the manager role.
        /// </summary>
        public static void AutoGenerateManagers()
        {
            // Get employees.
            List<Employee> managers = App.Helios.StaffReader.Managers();
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
                default:
                    break;
            }
        }

        private void LoadAion()
        {
            if (Aion is null) Aion = new AionPage();
            SetPage(Aion);
        }

        private void LoadPrometheus()
        {
            if (Prometheus is null) Prometheus = new PrometheusPage();
            SetPage(Prometheus);
        }

        private void LoadPantheon()
        {
            if (Pantheon is null) Pantheon = new PantheonPage();
            SetPage(Pantheon);
        }

        private void LoadVulcan()
        {
            if (Vulcan is null) Vulcan = new VulcanPage();
            SetPage(Vulcan);
        }

        private void LoadTorch()
        {
            if (Phoenix is null) Phoenix = new PhoenixPage();
            SetPage(Phoenix);
        }

        private void LoadKhaos()
        {
            if (Khaos is null) Khaos = new KhaosPage();
            SetPage(Khaos);
        }

        private void SetPage(IProject project)
        {
            Page page = project as Page;
            if (CurrentPage is null)
                CurrentPage = page;
            else
                _ = CurrentPage.NavigationService.Navigate(page);
        }

        public static List<SKUMaster> GetMasters()
        {
            // TODO: Change to run all DB actions in a single transaction.
            // Set tasks to pull data from db.
            Task<List<NAVItem>> getItemsTask = Task.Run(() => App.Helios.InventoryReader.NAVItems());
            Task<Dictionary<int, List<NAVStock>>> getStockTask = Task.Run(() => App.Helios.InventoryReader.NAVAllStock()
                .GroupBy(s => s.ItemNumber)
                .ToDictionary(g => g.Key, g => g.ToList()));
            Task<Dictionary<int, Dictionary<string, NAVUoM>>> getUoMTask = Task.Run(() => App.Helios.InventoryReader.NAVUoMs()
                .GroupBy(u => u.ItemNumber)
                .ToDictionary(g => g.Key, g => g.ToDictionary(u => u.Code, u => u)));
            Task<Dictionary<string, NAVBin>> getBinsTask = Task.Run(() => App.Helios.InventoryReader.NAVBins().ToDictionary(b => b.ID, b => b));
            Task<Dictionary<int, string>> getDivsTask = Task.Run(() => App.Helios.InventoryReader.NAVDivisions().ToDictionary(d => d.Code, d => d.Description));
            Task<Dictionary<int, string>> getCatsTask = Task.Run(() => App.Helios.InventoryReader.NAVCategorys().ToDictionary(c => c.Code, c => c.Description));
            Task<Dictionary<int, string>> getPFsTask = Task.Run(() => App.Helios.InventoryReader.NAVPlatforms().ToDictionary(p => p.Code, p => p.Description));
            Task<Dictionary<int, string>> getGensTask = Task.Run(() => App.Helios.InventoryReader.NAVGenres().ToDictionary(g => g.Code, g => g.Description));
            // Wait for tasks to complete.
            Task.WaitAll(getBinsTask, getCatsTask, getDivsTask, getGensTask, getItemsTask, getPFsTask, getStockTask, getUoMTask);
            // Assign results to data lists/dicts.
            List<NAVItem> navItems = getItemsTask.Result;
            Dictionary<int, List<NAVStock>> stock = getStockTask.Result;
            Dictionary<int, Dictionary<string, NAVUoM>> uoms = getUoMTask.Result;
            Dictionary<string, NAVBin> bins = getBinsTask.Result;
            Dictionary<int, string> divisions = getDivsTask.Result;
            Dictionary<int, string> categories = getCatsTask.Result;
            Dictionary<int, string> platforms = getPFsTask.Result;
            Dictionary<int, string> genres = getGensTask.Result;
            // Generate Sku Master List
            List<SKUMaster> masters = new();
            foreach (var item in navItems)
            {
                masters.Add(new SKUMaster(item, stock, uoms, bins, divisions, categories, platforms, genres));
            }
            return masters;
        }

        private static void EstablishInitialProjectIcons()
        {
            App.Helios.StaffCreator.CopyProjectIconsFromSource(Path.Combine(App.BaseDirectory(), "Resources", "Images", "Icons"));
            List<Project> projects = new()
            {
                new Project(EProject.Khaos, "chaos.ico", App.Helios.StaffReader,"Handles makebulk designation and separation. (Genesis)"),
                new Project(EProject.Pantheon, "pantheon.ico", App.Helios.StaffReader, "Roster management."),
                new Project(EProject.Prometheus, "prometheus.ico", App.Helios.StaffReader, "Data management."),
                new Project(EProject.Phoenix, "phoenix.ico", App.Helios.StaffReader, "Pre-work automated stock maintenance. (AutoBurn)"),
                new Project(EProject.Vulcan, "vulcan.ico", App.Helios.StaffReader, "Replenishment DDR management and work assignment. (RefOrge)"),
                new Project(EProject.Aion, "Aion.ico", App.Helios.StaffReader, "Employee clock in and shift time managemnet.")
            };

            App.Helios.StaffCreator.EstablishInitialProjects(projects);
            
        }

        public static void GenerateMasterSkuList()
        {
            List<SKUMaster> masters = GetMasters();

            // Make sure the target destination exists.
            string dirPath = Path.Combine(App.BaseDirectory(), "SKUMasterExports");

            Directory.CreateDirectory(dirPath);

            Task csvTask = Task.Run(() => ExportMasterSkusAsCSV(masters, dirPath));
            Task jsonTask = Task.Run(() => ExportMasterSkusAsJSON(masters, dirPath));
            Task xmlTask = Task.Run(() => ExportMasterSkusAsXML(masters, dirPath));
            Task sqlTask = Task.Run(() => ExportMasterSkusIntoSQLite(masters, dirPath));
            Task.WaitAll(csvTask, jsonTask, xmlTask, sqlTask);

            _ = MessageBox.Show("Files exported.");
        }

        public static void ExportMasterSkusAsCSV(List<SKUMaster> masters, string dirPath)
        {
            string csv = CsvSerializer.SerializeToCsv(masters);
            string filePath = Path.Combine(dirPath, "SKUMasterExport.csv");
            File.WriteAllText(filePath, csv);
        }

        public static void ExportMasterSkusAsJSON(List<SKUMaster> masters, string dirPath)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(masters);
            string filePath = Path.Combine(dirPath, "SKUMasterExport.json");
            File.WriteAllText(filePath, json);
        }

        public static void ExportMasterSkusIntoSQLite(List<SKUMaster> masters, string dirPath)
        {
            string dbPath = Path.Combine(dirPath, "SKUMasterExport.sqlite");
            using SQLiteConnection database = new(dbPath);
            _ = database.CreateTable(typeof(SKUMaster));
            _ = database.DeleteAll<SKUMaster>();
            _ = database.InsertAll(masters);
        }

        public static void ExportMasterSkusAsXML(List<SKUMaster> masters, string dirPath)
        {
            string xml = XmlSerializer.SerializeToString(masters);
            string filePath = Path.Combine(dirPath, "SKUMasterExport.xml");
            File.WriteAllText(filePath, xml);
        }
    }
}
