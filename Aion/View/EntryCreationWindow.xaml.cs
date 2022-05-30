using Aion.ViewModels;
using Uranus;

namespace Aion.View;

/// <summary>
/// Interaction logic for SimpleEntryCreationView.xaml
/// </summary>
public partial class EntryCreationWindow
{
    public EntryCreationWindow(Helios helios, ShiftEntryPageVM editorVM)
    {
        InitializeComponent();
        VM.SetData(helios, editorVM);
    }
}