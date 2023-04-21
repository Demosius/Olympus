using Deimos.ViewModels;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Deimos.Views;

/// <summary>
/// Interaction logic for DeimosPage.xaml
/// </summary>
public partial class DeimosPage : IProject
{
    public DeimosVM VM { get; set; }

    public DeimosPage(Helios helios)
    {
        VM = new DeimosVM(helios);
        InitializeComponent();
        DataContext = VM;
    }

    public EProject Project => EProject.Deimos;

    public static bool RequiresUser => false;

    public void RefreshData()
    {
        throw new System.NotImplementedException();
    }
}