using Styx;
using System.Windows.Input;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Aion.View;

/// <summary>
/// Interaction logic for ManagerView.xaml
/// </summary>
public partial class AionPage : IProject
{
    public AionPage(Helios helios, Charon charon)
    {
        InitializeComponent();

        KeyGesture backKeyGesture = null;
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

        VM.SetDataSources(helios, charon);
    }

    public EProject Project => EProject.Aion;

    public static bool RequiresUser => true;

    public void RefreshData()
    {
        VM.RefreshData();
    }
}