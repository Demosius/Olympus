using Argos.ViewModels.PopUps;
using Uranus.Inventory.Models;

namespace Argos.Views.PopUps;

/// <summary>
/// Interaction logic for FillProgressSelectionWindow.xaml
/// </summary>
public partial class FillProgressSelectionWindow
{
    public FillProgressSelectionVM VM { get; set; }
    public EBatchFillProgress Progress => VM.FillProgress;

    public FillProgressSelectionWindow(string prompt)
    {
        VM = new FillProgressSelectionVM(prompt);
        InitializeComponent();
        DataContext = VM;
    }
}