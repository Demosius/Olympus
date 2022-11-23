# Olympus
All in one EB DC internal application(s).

## Adding New Project Checklist:
1. Add Project to Uranus.
    - Staff.EnumConverter.***EProject*** (and EnumConverter)
2. Add Project reference to Uranus (from new Project) (and other required Projects - such as Styx and Morpheus)
3. Add project folders.
    - Models
    - Views
    - ViewModels
4. Add Views.
    - ProjectNamePage.xaml
      - IProject Interface to ProjectNamePage.xaml.cs
      - Set Project as appropriate.
      - Set RequiresUser value based on if there is sensitive information within project.
    - ProjectNameWindow.xaml
5. Add ViewModels.
    - ProjectNameVM.cs
    - AppVM.cs
6. Change App.xaml StartupUri.
    > StartupUri="Views/ProjectName.xaml">
7. Add project to Olympus.
    - Add Project reference.
    - Add appropriate icon (.ico) to Resources.Images.Icons
      - Build Action Content
      - Copy if newer
    - OlympusVM.EstablishInitialProjectIcons
    - ViewModels.Utility.ProjectFactory.GetProject
    - ViewModels.Utility.ProjectFactory.RequiresUser
    
    
## Project Ideas to Be Implemented

### Cadmus
    - Greek god of writing.
    - For drawing/creating and printing labels and documents.
    - Usable directly and anticipated to be called on by other projects.

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
    - Purpose is to ensure everyone has access to the same information regarding where each department is up to with batches.
    - Also allows for a more streamlined and useful interface compared to tracking batches in NAV.
    
