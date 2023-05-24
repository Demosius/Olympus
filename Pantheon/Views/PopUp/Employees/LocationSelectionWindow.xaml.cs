using System;
using System.Windows;
using Pantheon.ViewModels.PopUp.Employees;
using Styx;
using Uranus;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for LocationSelectionWindow.xaml
/// </summary>
public partial class LocationSelectionWindow
{
    public LocationSelectionVM? VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public LocationSelectionWindow(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        InitializeComponent();
    }

    private async void LocationSelectionWindow_OnInitialized(object? sender, EventArgs e)
    {
        VM = await LocationSelectionVM.CreateAsync(Helios, Charon);
        DataContext = VM;
    }
}