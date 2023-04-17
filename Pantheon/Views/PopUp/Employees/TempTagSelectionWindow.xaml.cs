using Pantheon.ViewModels.PopUp.TempTags;
using Styx;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for TempTagSelectionWindow.xaml
/// </summary>
public partial class TempTagSelectionWindow
{
    public TempTagSelectionVM VM { get; set; }

    public TempTag? TempTag => VM.SelectedTag?.TempTag;

    public TempTagSelectionWindow(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM = new TempTagSelectionVM(helios, charon);
        DataContext = VM;
    }
}