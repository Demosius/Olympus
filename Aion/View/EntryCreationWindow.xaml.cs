using Aion.ViewModels;
using Uranus;

namespace Aion.View;

/// <summary>
/// Interaction logic for SimpleEntryCreationView.xaml
/// </summary>
public partial class EntryCreationWindow
{
    public EntryCreationVM VM { get; set; }

    public EntryCreationWindow(Helios helios, ShiftEntryPageVM editorVM)
    {
        VM = new EntryCreationVM(helios, editorVM);
        InitializeComponent();
        DataContext = VM;
    }
}