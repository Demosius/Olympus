using System;
using System.Windows;
using Pantheon.ViewModels.PopUp.Rosters;
using Styx;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.Views.PopUp.Rosters;

/// <summary>
/// Interaction logic for RosterCreationWindow.xaml
/// </summary>
public partial class RosterCreationWindow
{
    public RosterCreationVM? VM { get; set; }
    public Department Department { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public RosterCreationWindow(Department department, Helios helios, Charon charon)
    {
        Department = department;
        Helios = helios;
        Charon = charon;
        InitializeComponent();
    }

    private async void RosterCreationWindow_OnInitialized(object? sender, EventArgs e)
    {
        VM = await RosterCreationVM.CreateAsync(Department, Helios, Charon);
        DataContext = VM;
    }
}