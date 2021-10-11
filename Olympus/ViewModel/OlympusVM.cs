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

        public void GenerateMasterSkuList(bool useRecursion)
        {
            Stopwatch tim = new Stopwatch();

            tim.Start();
            List<NAVItem> navItems = App.Helios.InventoryReader.NAVItems();
            Console.WriteLine($"Get Base Items: {tim.ElapsedMilliseconds} ms"); tim.Restart();
            Dictionary<int, List<NAVStock>> stock = App.Helios.InventoryReader.NAVAllStock()
                                                        .GroupBy(s => s.ItemNumber)
                                                        .ToDictionary(g => g.Key, g => g.ToList());
            Console.WriteLine($"Get Base Stock: {tim.ElapsedMilliseconds} ms"); tim.Restart();
            Dictionary<int, Dictionary<string, NAVUoM>> uoms = App.Helios.InventoryReader.NAVUoMs()
                                                                .GroupBy(u => u.ItemNumber)
                                                                .ToDictionary(g => g.Key, g => g.ToDictionary(u => u.Code, u => u));
            Console.WriteLine($"Get Base UoMs: {tim.ElapsedMilliseconds} ms"); tim.Restart();
            Dictionary<string, NAVBin> bins = App.Helios.InventoryReader.NAVBins().ToDictionary(b => b.ID, b => b);
            Console.WriteLine($"Get Base Bins: {tim.ElapsedMilliseconds} ms"); tim.Restart();
            Dictionary<int, string> divisions = App.Helios.InventoryReader.NAVDivisions().ToDictionary(d => d.Code, d => d.Description);
            Dictionary<int, string> categories = App.Helios.InventoryReader.NAVCategorys().ToDictionary(c => c.Code, c => c.Description);
            Dictionary<int, string> platforms = App.Helios.InventoryReader.NAVPlatforms().ToDictionary(p => p.Code, p => p.Description);
            Dictionary<int, string> genres = App.Helios.InventoryReader.NAVGenres().ToDictionary(g => g.Code, g => g.Description);
            Console.WriteLine($"Get Div/Cat/PF/Gens: {tim.ElapsedMilliseconds} ms"); tim.Restart();
            List<SKUMaster> masters = new List<SKUMaster>();
            foreach (var item in navItems)
            {
                masters.Add(new SKUMaster(item, stock, uoms, bins, divisions, categories, platforms, genres));
            }
            Console.WriteLine($"Set Master Skus: {tim.ElapsedMilliseconds} ms"); tim.Restart();
            // Make sure the target destination is valid.
            string dirPath = Path.Combine(App.BaseDirectory(), "SKUMasterExports");
            Directory.CreateDirectory(dirPath);
            Console.WriteLine($"Establish Dir: {tim.ElapsedMilliseconds} ms"); tim.Restart();
            ExportMasterSkusAsCSV(masters, dirPath);
            Console.WriteLine($"Generate CSV: {tim.ElapsedMilliseconds} ms"); tim.Restart();
            ExportMasterSkusAsJSON(masters, dirPath);
            Console.WriteLine($"Generate JSON: {tim.ElapsedMilliseconds} ms"); tim.Restart();
            //ExportMasterSkusAsXML(masters, dirPath);
            Console.WriteLine($"Generate XML: {tim.ElapsedMilliseconds} ms"); tim.Restart();
            ExportMasterSkusIntoSQLite(masters, dirPath);
            Console.WriteLine($"Generate SQLite: {tim.ElapsedMilliseconds} ms"); tim.Stop();

            MessageBox.Show("Files exported.");
        }

        public void GenerateMasterSkuList()
        {
            Stopwatch tim = new Stopwatch();

            tim.Start();
            Console.WriteLine("GETTING ITEMS (Object Only)");
            List<NAVItem> navItems = App.Helios.InventoryReader.NAVItems(null, Uranus.PullType.ObjectOnly).ToList();
            Console.WriteLine($"Get Items: {tim.ElapsedMilliseconds} ms - {navItems.Count} items"); tim.Restart();
            Console.WriteLine("GETTING ITEMS (Include Children)");
            navItems = App.Helios.InventoryReader.NAVItems(null, Uranus.PullType.IncludeChildren).ToList();
            Console.WriteLine($"Get Items: {tim.ElapsedMilliseconds} ms - {navItems.Count} items"); tim.Restart();
            return;
            Console.WriteLine("GETTING ITEMS (Full Recursive) WHERE CATAGORYCODE == 999");
            navItems = App.Helios.InventoryReader.NAVItems(i => i.CategoryCode == 999, Uranus.PullType.FullRecursive).ToList();
            Console.WriteLine($"Get Items: {tim.ElapsedMilliseconds} ms - {navItems.Count} items"); tim.Restart();
            List<SKUMaster> masters = new List<SKUMaster>();
            foreach (var item in navItems)
            {
                masters.Add(new SKUMaster(item));
                if (masters.Count % 1000 == 0)
                    Console.WriteLine(masters.Count);
            }
            Console.WriteLine($"Get Master SKU: {tim.ElapsedMilliseconds} ms"); tim.Restart();
            // Make sure the target destination is valid.
            string dirPath = Path.Combine(App.BaseDirectory(), "SKUMasterExports");
            Directory.CreateDirectory(dirPath);
            Console.WriteLine($"Generate Dir: {tim.ElapsedMilliseconds} ms"); tim.Restart();

            ExportMasterSkusAsCSV(masters, dirPath);
            Console.WriteLine($"Generate CSV: {tim.ElapsedMilliseconds} ms"); tim.Restart();
            ExportMasterSkusAsJSON(masters, dirPath);
            Console.WriteLine($"Generate JSON: {tim.ElapsedMilliseconds} ms"); tim.Restart();
            //ExportMasterSkusAsXML(masters, dirPath);
            Console.WriteLine($"Generate XML: {tim.ElapsedMilliseconds} ms"); tim.Restart();
            ExportMasterSkusIntoSQLite(masters, dirPath);
            Console.WriteLine($"Generate SQLite: {tim.ElapsedMilliseconds} ms"); tim.Restart();
            Console.WriteLine(masters.Count());
            Console.WriteLine($"END: {tim.ElapsedMilliseconds} ms"); tim.Stop();
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
