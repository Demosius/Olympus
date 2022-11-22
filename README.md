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
    
    
  
