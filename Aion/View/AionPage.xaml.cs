using System.Threading.Tasks;
using Styx;
using System.Windows.Input;
using Aion.ViewModels;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Aion.View;

/// <summary>
/// Interaction logic for ManagerView.xaml
/// </summary>
public partial class AionPage : IProject
{
    public AionVM VM { get; set; }

    public AionPage(Helios helios, Charon charon)
    {
        VM = new AionVM(helios, charon);

        InitializeComponent();

        DataContext = VM;

        KeyGesture? backKeyGesture = null;
        foreach (InputGesture browseBackInputGesture in NavigationCommands.BrowseBack.InputGestures)
        {
            if (browseBackInputGesture is KeyGesture { Key: Key.Back, Modifiers: ModifierKeys.None } keyGesture)
            {
                backKeyGesture = keyGesture;
            }
        }

        if (backKeyGesture != null)
        {
            NavigationCommands.BrowseBack.InputGestures.Remove(backKeyGesture);
        }
    }

    public EProject Project => EProject.Aion;

    public static bool RequiresUser => true;

    public async Task RefreshDataAsync()
    {
        await VM.RefreshDataAsync();
    }
}