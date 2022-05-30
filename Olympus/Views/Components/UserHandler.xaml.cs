using Olympus.Properties;
using Styx;

namespace Olympus.Views.Components;

/// <summary>
/// Interaction logic for SignIn.xaml
/// </summary>
public partial class UserHandler
{
    public Charon Charon { get; set; }

    public UserHandler()
    {
        InitializeComponent();
        Charon = new Charon(Settings.Default.SolLocation);
    }
}