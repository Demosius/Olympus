using Argos.ViewModels.PopUps;
using Uranus;

namespace Argos.Views.PopUps;

/// <summary>
/// Interaction logic for FileLocationMenuWindow.xaml
/// </summary>
public partial class FileLocationMenuWindow
{
    public FileLocationMenuVM VM { get; set; }

    public FileLocationMenuWindow(Helios helios)
    {
        VM = new FileLocationMenuVM(helios);
        InitializeComponent();
        DataContext = VM;
    }
}