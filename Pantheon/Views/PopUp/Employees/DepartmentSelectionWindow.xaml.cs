using System;
using System.Windows;
using Pantheon.ViewModels.PopUp.Employees;
using Styx;
using Uranus;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for DepartmentSelectionWindow.xaml
/// </summary>
public partial class DepartmentSelectionWindow
{
    public DepartmentSelectionVM? VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public DepartmentSelectionWindow(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        InitializeComponent();
    }

    private async void DepartmentSelectionWindow_OnInitialized(object? sender, EventArgs e)
    {
        VM = await DepartmentSelectionVM.CreateAsync(Helios, Charon);
        DataContext = VM;
    }
}