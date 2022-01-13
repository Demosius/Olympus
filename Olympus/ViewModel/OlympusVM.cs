using Olympus.Torch.View;
using Olympus.Uranus.Staff;
using Olympus.Pantheon.View;
using Olympus.Prometheus.View;
using Olympus.Vulcan.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Olympus.Khaos.View;
using Olympus.ViewModel.Components;
using Olympus.Model;
using Olympus.ViewModel.Commands;
using Olympus.Uranus.Inventory.Model;
using System.IO;
using ServiceStack.Text;
using ServiceStack.Extensions;
using SQLite;
using Olympus.Uranus.Inventory;
using System.Diagnostics;
using System.Windows;
using System.Data;

namespace Olympus.ViewModel
{

    public class OlympusVM : INotifyPropertyChanged
    {
        public PrometheusPage Prometheus { get; set; }
        public PantheonPage Pantheon { get; set; }
        public VulcanPage Vulcan { get; set; }
        public TorchPage Torch { get; set; }
        public KhaosPage Khaos { get; set; }

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
        public DBSelectionVM DBSelectionVM { get; set; }
        public InventoryUpdaterVM InventoryUpdaterVM { get; set; }
        public ProjectLauncherVM ProjectLauncherVM { get; set; }
        public UserHandlerVM UserHandlerVM { get; set; }

        /* Commands */
        public GenerateMasterSkuListCommand GenerateMasterSkuListCommand { get; set; }

        /* Constructor(s) */
        public OlympusVM()
        {
            DBSelectionVM = new DBSelectionVM(this);
            UserHandlerVM = new UserHandlerVM(this);
            ProjectLauncherVM = new ProjectLauncherVM(this);
            InventoryUpdaterVM = new InventoryUpdaterVM(this);

            GenerateMasterSkuListCommand = new GenerateMasterSkuListCommand(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                case EProject.Torch:
                    LoadTorch();
                    break;
                case EProject.Khaos:
                    LoadKhaos();
                    break;
                default:
                    break;
            }
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
            if (Torch is null) Torch = new TorchPage();
            SetPage(Torch);
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
                CurrentPage.NavigationService.Navigate(page);
        }

        public List<SKUMaster> GetMasters()
        {
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
            List<SKUMaster> masters = new List<SKUMaster>();
            foreach (var item in navItems)
            {
                masters.Add(new SKUMaster(item, stock, uoms, bins, divisions, categories, platforms, genres));
            }
            return masters;
        }

        public void GenerateMasterSkuList()
        {
            List<SKUMaster> masters = GetMasters();

            // Make sure the target destination exists.
            string dirPath = Path.Combine(App.BaseDirectory(), "SKUMasterExports");

            Task csvTask = Task.Run(() => ExportMasterSkusAsCSV(masters, dirPath));
            Task jsonTask = Task.Run(() => ExportMasterSkusAsJSON(masters, dirPath));
            Task xmlTask = Task.Run(() => ExportMasterSkusAsXML(masters, dirPath));
            Task sqlTask = Task.Run(() => ExportMasterSkusIntoSQLite(masters, dirPath));
            Task.WaitAll(csvTask, jsonTask, xmlTask, sqlTask);

            MessageBox.Show("Files exported.");
        }

        public void ExportMasterSkusAsCSV(List<SKUMaster> masters, string dirPath)
        {
            string csv = CsvSerializer.SerializeToCsv(masters);
            string filePath = Path.Combine(dirPath, "SKUMasterExport.csv");
            File.WriteAllText(filePath, csv);
        }

        public void ExportMasterSkusAsJSON(List<SKUMaster> masters, string dirPath)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(masters);
            string filePath = Path.Combine(dirPath, "SKUMasterExport.json");
            File.WriteAllText(filePath, json);
        }

        public void ExportMasterSkusIntoSQLite(List<SKUMaster> masters, string dirPath)
        {
            string dbPath = Path.Combine(dirPath, "SKUMasterExport.sqlite");
            using (SQLiteConnection database = new SQLiteConnection(dbPath))
            {
                database.CreateTable(typeof(SKUMaster));
                database.DeleteAll<SKUMaster>();
                database.InsertAll(masters);
            }
        }

        public void ExportMasterSkusAsXML(List<SKUMaster> masters, string dirPath)
        {
            string xml = XmlSerializer.SerializeToString(masters);
            string filePath = Path.Combine(dirPath, "SKUMasterExport.xml");
            File.WriteAllText(filePath, xml);
        }
    }
}
