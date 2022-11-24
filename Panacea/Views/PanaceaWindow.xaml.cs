using Panacea.Properties;

namespace Panacea.Views;

/// <summary>
/// Interaction logic for PanaceaWindow.xaml
/// </summary>
public partial class PanaceaWindow
{
    public PanaceaWindow()
    {
        InitializeComponent();
    }

    private void PanaceaWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        // Saves the settings when closing the App.
        Settings.Default.Save();
    }
}