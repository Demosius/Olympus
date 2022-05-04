using System;
using System.Collections.Generic;
using Styx;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Uranus;

namespace Prometheus.View.Pages.Users;

/// <summary>
/// Interaction logic for UserDeactivatePage.xaml
/// </summary>
public partial class UserDeactivatePage
{
    public UserDeactivatePage(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM.SetDataSources(helios, charon);
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