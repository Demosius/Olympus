using Styx;
using System.Windows.Controls;
using Prometheus.ViewModels.Pages.Users;
using Uranus;

namespace Prometheus.Views.Pages.Users;

/// <summary>
/// Interaction logic for UserDeactivatePage.xaml
/// </summary>
public partial class UserDeactivatePage
{
    public RolesVM VM { get; set; }

    public UserDeactivatePage(Helios helios, Charon charon)
    {
        VM = new RolesVM(helios, charon);
        InitializeComponent();
        DataContext = VM;
    }

    private void RoleGrid_OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        var headerName = e.Column.Header.ToString();

        switch (headerName)
        {
            case "Users":
            case "UserPermissionsTotal":
            case "Description":
                e.Cancel = true;
                break;
            case "Name":
                e.Column.IsReadOnly = true;
                break;
        }
    }
}