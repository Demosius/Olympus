using Uranus.Interfaces;
using Uranus.Staff;

namespace Vulcan.Views;

/// <summary>
/// Interaction logic for VulcanPage.xaml
/// </summary>
public partial class VulcanPage : IProject
{
    public VulcanPage()
    {
        InitializeComponent();
    }

    public EProject Project => EProject.Vulcan;

    public static bool RequiresUser => false;

    public void RefreshData()
    {
        VM.RefreshData();
    }
}