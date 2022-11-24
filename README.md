# Olympus
All in one EB DC internal application(s).

## Adding New Project Checklist:
1. Add new project to Olympus
    - Right Click => Add => New Project...
        - QPF Application (C#)
            - ProjectName
                - .Net 6.0 (Long-term support)
1. Add Project to Uranus.
    - Staff.EnumConverter.***EProject*** (and EnumConverter)
1. Add Project reference to Uranus (from new Project) (and other required Projects - such as Styx and Morpheus)
1. Add project folders.
    - Models
    - Views
    - ViewModels
1. Add SolLocation in Settings - if required.
    - Right click project => Properties (Alt+Enter)
        - Settings
            - Create or open applicatino sttings
                - Name: SolLocation
                - Type: String
                - Scope: User
                - Value: Default database location, i.e. "\\ausefpdfs01ns\Shares\Public\DC_Data\Olympus\QA\Sol"
1. Add interaction logic for Existing App.xaml (in App.xaml.cs).
    - Add Charon and Helios if appropriate, referencing the SolLocation in settings.
    ```    
    public static Charon Charon { get; set; } = new(Settings.Default.SolLocation);
    public static Helios Helios { get; set; } = new(Settings.Default.SolLocation);
    ```
    - Add BaseDirectory:
    ```
    public static string BaseDirectory()
    {
        return Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
    }
    ```
1. Add Views.
    - ProjectNamePage.xaml
    - ProjectNameWindow.xaml
1. Add ViewModels.
    - ProjectNameVM.cs
        - Public
        - Add Interfaces as necessary:
            - INotifyPropertyChanged
            - IDBInteraction
            - IDataSource
        - Add Helios and Charon to constructor if necessary - adding also Helios as property if not already included with IDataSource.
    - AppVM.cs
        - Public
        - Add INotifyPropertyChanged interface.
        - Add project page - including Helios and Charon as necessary:
        ```
        public ProjectNamePage ProjectNamePage { get; set; }

        public AppVM()
        {
            ProjectNamePage = new ProjectNamePage(App.Helios, App.Charon); 
        }
        ```
1. Edit Views.
    - Copy Themes and Resources (at least such that contains Fonts.xaml and Fonts subfolder) folders from another project to use - and edit as desired.
    - ProjectNamePage.xaml
        - Add page resources:
        ```
        <Page.Resources>
            <ResourceDictionary>
                <ResourceDictionary.MergedDictionaries>
                    <ResourceDictionary Source="../Themes/MSControls.Core.Implicit.xaml"/>
                </ResourceDictionary.MergedDictionaries>
            </ResourceDictionary>
        </Page.Resources>
        ```
    - ProjectNamePage.xaml.cs
        - Add IProject Interface
        - Add Helios and Charon to constructor as necessary.
        ```
        public ProjectNamePage(Helios helios, Charon charon)
        {
            InitializeComponent();
            DataContext = new ProjectNameVM(helios, charon);
        }
        ```
        - Set Project as appropriate.
        - Set RequiresUser value based on if there is sensitive information within project.
       
    - ProjectNameWindow.xaml
        - Add datacontext:
        `xmlns:viewModels="clr-namespace:ProjectName.ViewModels"`
        ...
        ```
        <Window.DataContext>
            <viewModels:AppVM x:Name="VM"/>
        </Window.DataContext>
        ```
        - Add window recources as required.
        ```
        <Window.Resources>
            <ResourceDictionary>
                <ResourceDictionary.MergedDictionaries>
                    <ResourceDictionary Source="../Themes/MSControls.Core.Implicit.xaml"/>
                </ResourceDictionary.MergedDictionaries>
            </ResourceDictionary>
        </Window.Resources>
        ```
        - Add frame for project page.
        ```
        <Frame Source="{Binding ProjectNamePage}"
           Margin="0"
           Height="Auto" 
           HorizontalAlignment="Stretch" 
           VerticalAlignment="Stretch"
           BorderThickness="1" 
           BorderBrush="SlateGray"
           NavigationUIVisibility="Hidden"/>
        ```
1. Change App.xaml StartupUri.
    > StartupUri="Views/ProjectNameWindow.xaml">
1. Delete default MainWindow.
1. Add project to Olympus.
    - Add Project reference.
    - Add appropriate icon (.ico) to Resources.Images.Icons
      - Build Action Content
      - Copy if newer
    - ViewModels.OlympusVM.EstablishInitialProjectIcons
    - ViewModels.Utility.ProjectFactory.GetProject
    - ViewModels.Utility.ProjectFactory.RequiresUser
    
    
## Project Ideas to Be Implemented

### Cadmus
    - Greek god of writing.
    - For drawing/creating and printing labels and documents.
    - Usable directly and anticipated to be called on by other projects.

### Sphynx
    - About solving riddles.
    - Tracks and assists with ongoing stock issues.
    - Replacement for Countables and Product Seach Sheet.
    
### Panacea 
    - Godess of Universal Remedy
    - AllFix Equivalent
    - Include multiple sub-functionality.
    - Add the fixed bin checker under this umbrella.
    
### Hades
    - (NIMS) Non-Inventory Management system.
    - Still open to thematic name.
    - For tracking non-inventory in the warehouse.
    - Start with Cardboard and see about adoption rate before attempting to integrate other stock/staff/departments/processes.
    
### Unified Batch Checker
    - Still needs name.
        -  Argus Panoptes (all-seeing) / Argos 
    - Purpose is to ensure everyone has access to the same information regarding where each department is up to with batches.
    - Also allows for a more streamlined and useful interface compared to tracking batches in NAV.
    
