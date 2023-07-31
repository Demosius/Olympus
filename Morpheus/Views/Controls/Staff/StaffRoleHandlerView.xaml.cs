using System;
using Morpheus.ViewModels.Controls.Staff;
using Styx;
using Uranus;

namespace Morpheus.Views.Controls.Staff;

/// <summary>
/// Interaction logic for StaffRoleHandlerView.xaml
/// </summary>
public partial class StaffRoleHandlerView
{
    public StaffRoleHandlerVM VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public StaffRoleHandlerView(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        VM = StaffRoleHandlerVM.CreateEmpty(Helios, Charon);
        InitializeComponent();
        DataContext = VM;
    }

    private async void StaffRoleHandlerView_OnInitialized(object? sender, EventArgs e)
    {
        VM = await StaffRoleHandlerVM.CreateAsync(Helios, Charon);
        DataContext = VM;
    }
}