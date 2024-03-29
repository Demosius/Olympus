﻿using Cadmus.Annotations;
using Olympus.Properties;
using Olympus.ViewModels.Commands;
using Olympus.ViewModels.Components;
using Olympus.ViewModels.Utility;
using Olympus.Views;
using ServiceStack.Text;
using SQLite;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Morpheus.ViewModels.Controls;
using Uranus.Interfaces;
using Uranus.Inventory.Models;
using Uranus.Staff;
using Uranus.Staff.Models;

namespace Olympus.ViewModels;

public class OlympusVM : INotifyPropertyChanged
{
    public Dictionary<EProject, IProject> RunningProjects { get; set; }

    private IProject? currentProject;
    public IProject? CurrentProject
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
    public ProjectLauncherVM ProjectLauncherVM { get; set; } = null!;
    public UserHandlerVM UserHandlerVM { get; set; }
    public ProgressBarVM ProgressBarVM { get; set; }

    /* Commands */
    public GenerateMasterSkuListCommand GenerateMasterSkuListCommand { get; set; }
    public ChangePasswordCommand ChangePasswordCommand { get; set; }

    /* Constructor(s) */
    private OlympusVM()
    {
        RunningProjects = new Dictionary<EProject, IProject>();

        EstablishInitialProjectIcons();

        DBManager = new DBManager(this);
        UserHandlerVM = new UserHandlerVM(this);
        InventoryUpdaterVM = new InventoryUpdaterVM(this);
        ProgressBarVM = App.ProgressBar;

        GenerateMasterSkuListCommand = new GenerateMasterSkuListCommand(this);
        ChangePasswordCommand = new ChangePasswordCommand();
    }

    private async Task<OlympusVM> InitializeAsync()
    {
        ProjectLauncherVM = await ProjectLauncherVM.CreateAsync(this);
        return this;
    }

    public static Task<OlympusVM> CreateAsync()
    {
        var ret = new OlympusVM();
        return ret.InitializeAsync();
    }

    internal async Task RefreshData()
    {
        var tasks = RunningProjects.Select(project => project.Value.RefreshDataAsync()).ToList();
        await Task.WhenAll(tasks);
    }

    internal async Task ResetDB()
    {
        App.Helios.ResetChariots(Settings.Default.SolLocation);
        App.Charon.DatabaseReset(Settings.Default.SolLocation);

        EstablishInitialProjectIcons();

        UserHandlerVM.CheckUser();
        ProjectLauncherVM = await ProjectLauncherVM.CreateAsync(this);
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
            if (loadedProject is not null)
                RunningProjects.Add(project, loadedProject);
        }
        if (loadedProject is not null) SetPage(loadedProject);
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

    public static void LaunchPasswordChanger()
    {
        var changePasswordWindow = new ChangePasswordWindow(App.Charon);
        changePasswordWindow.ShowDialog();
    }

    private static void EstablishInitialProjectIcons()
    {
        App.Helios.StaffCreator.CopyProjectIconsFromSource(Path.Combine(App.BaseDirectory(), "Resources", "Images", "Icons"));
        List<Project> projects = new()
        {
            new Project(EProject.Pantheon, "pantheon.ico", App.Helios.StaffReader, "Roster management."),
            new Project(EProject.Prometheus, "prometheus.ico", App.Helios.StaffReader, "Data management."),
            new Project(EProject.Aion, "Aion.ico", App.Helios.StaffReader, "Employee clock in and shift time management."),
            new Project(EProject.Hydra, "Hydra.ico", App.Helios.StaffReader, "Manage stock levels between different facilities."),
            new Project(EProject.Cadmus, "cadmus.ico", App.Helios.StaffReader, "Manages document creation and printing."),
            new Project(EProject.Sphynx, "Sphynx.ico", App.Helios.StaffReader, "Product Investigation: Countables and Product Search Sheet"),
            new Project(EProject.Argos, "UBC.ico", App.Helios.StaffReader, "Batch Management Tool"),
            new Project(EProject.Panacea, "Panacea.ico", App.Helios.StaffReader, "ALLFIX: Multiple tools to remedy many given issues."),
            new Project(EProject.Quest, "quest.ico", App.Helios.StaffReader, "Manage Restock Staff assignment and tracking. (ZLAM & PickRate Tracker)"),
            new Project(EProject.Deimos, "deimos.ico", App.Helios.StaffReader, "PickRate tracking and error allocation. (Dredd)"),
            new Project(EProject.Khaos, "chaos.ico", App.Helios.StaffReader, "Handles make-bulk designation and separation. (Genesis)"),
            new Project(EProject.Phoenix, "phoenix.ico", App.Helios.StaffReader, "Pre-work automated stock maintenance. (AutoBurn)"),
            new Project(EProject.Vulcan, "vulcan.ico", App.Helios.StaffReader, "Replenishment DDR management and work assignment. (RefOrge)"),
            new Project(EProject.Hades, "Hades.ico", App.Helios.StaffReader, "N.I.M.S. - Non-Inventory Management System"),
            new Project(EProject.Hermes, "Hermes.ico", App.Helios.StaffReader, "Messaging and B ug Reporting system."),
        };

        App.Helios.StaffCreator.EstablishInitialProjects(projects);

    }

    public static async Task GenerateMasterSkuList()
    {
        var masters = (await App.Helios.InventoryReader.GetMastersAsync()).ToList();
        
        // Make sure the target destination exists.
        var dirPath = Path.Combine(App.BaseDirectory(), "SKUMasterExports");

        Directory.CreateDirectory(dirPath);

        var csvTask = Task.Run(() => ExportMasterSkuAsCSV(masters, dirPath));
        var jsonTask = Task.Run(() => ExportMasterSkuAsJson(masters, dirPath));
        var xmlTask = Task.Run(() => ExportMasterSkuAsXml(masters, dirPath));
        //var sqlTask = Task.Run(() => ExportMasterSkuIntoSqLite(masters, dirPath));
        Task.WaitAll(csvTask, jsonTask, xmlTask);//, sqlTask);

        MessageBox.Show($"Files exported to {dirPath}.");
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

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}