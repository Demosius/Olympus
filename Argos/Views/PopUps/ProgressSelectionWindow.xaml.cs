using Argos.ViewModels.PopUps;
using Uranus.Inventory.Models;

namespace Argos.Views.PopUps;

/// <summary>
/// Interaction logic for ProgressSelectionWindow.xaml
/// </summary>
public partial class ProgressSelectionWindow
{
    public ProgressSelectionVM VM { get; set; }
    public EBatchProgress Progress => VM.BatchProgress;

    public ProgressSelectionWindow(string prompt)
    {
        VM = new ProgressSelectionVM(prompt);
        InitializeComponent();
        DataContext = VM;
    }
}