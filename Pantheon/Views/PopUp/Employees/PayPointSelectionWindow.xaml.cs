using Pantheon.ViewModels.PopUp.Employees;
using Styx;
using Uranus;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for PayPointSelectionWindow.xaml
/// </summary>
public partial class PayPointSelectionWindow
{
    public PayPointSelectionVM VM { get; set; }

    public PayPointSelectionWindow(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM = new PayPointSelectionVM(helios, charon);
        DataContext = VM;
    }
}