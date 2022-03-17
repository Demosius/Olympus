using Olympus.Properties;
using Olympus.View;
using Olympus.ViewModel.Commands;
using Olympus.ViewModel.Components;
using Olympus.ViewModel.Utility;
using ServiceStack.Text;
using SQLite;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Uranus.Interfaces;
using Uranus.Inventory.Model;
using Uranus.Staff;
using Uranus.Staff.Model;

namespace Olympus.ViewModel;

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
    public ChangePasswordCommand ChangePasswordCommand { get; set; }

    /* Constructor(s) */
    public OlympusVM()
    {
        RunningProjects = new Dictionary<EProject, IProject>();

        EstablishInitialProjectIcons();

        DBManager = new DBManager(this);
        UserHandlerVM = new UserHandlerVM(this);
        ProjectLauncherVM = new ProjectLauncherVM(this);
        InventoryUpdaterVM = new InventoryUpdaterVM(this);

        GenerateMasterSkuListCommand = new GenerateMasterSkuListCommand(this);
        ChangePasswordCommand = new ChangePasswordCommand(this);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        ProjectLauncherVM = new ProjectLauncherVM(this);
        InventoryUpdaterVM = new InventoryUpdaterVM(this);

        OnPropertyChanged(nameof(ProjectLauncherVM));
        OnPropertyChanged(nameof(InventoryUpdaterVM));

        CurrentProject = null;
        RunningProjects = new Dictionary<EProject, IProject>();
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

        var navigationService = ((Page)CurrentProject).NavigationService;
        if (navigationService != null)
            _ = navigationService.Navigate((Page)project);

    }

    public void ClearRunningProjects()
    {
        CurrentProject = null;
        RunningProjects = new Dictionary<EProject, IProject>();
    }

    public void LaunchPasswordChanger()
    {
        var changePasswordWindow = new ChangePasswordWindow(App.Charon);
        changePasswordWindow.ShowDialog();
    }

    private static void EstablishInitialProjectIcons()
    {
        App.Helios.StaffCreator.CopyProjectIconsFromSource(Path.Combine(App.BaseDirectory(), "Resources", "Images", "Icons"));
        List<Project> projects = new()
        {
            new Project(EProject.Khaos, "chaos.ico", App.Helios.StaffReader, "Handles make-bulk designation and separation. (Genesis)"),
            new Project(EProject.Pantheon, "pantheon.ico", App.Helios.StaffReader, "Roster management."),
            new Project(EProject.Prometheus, "prometheus.ico", App.Helios.StaffReader, "Data management."),
            new Project(EProject.Phoenix, "phoenix.ico", App.Helios.StaffReader, "Pre-work automated stock maintenance. (AutoBurn)"),
            new Project(EProject.Vulcan, "vulcan.ico", App.Helios.StaffReader, "Replenishment DDR management and work assignment. (RefOrge)"),
            new Project(EProject.Aion, "Aion.ico", App.Helios.StaffReader, "Employee clock in and shift time management.")
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
        var jsonTask = Task.Run(() => ExportMasterSkuAsJson(masters, dirPath));
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

    public static void ExportMasterSkuAsJson(IEnumerable<SkuMaster> masters, string dirPath)
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