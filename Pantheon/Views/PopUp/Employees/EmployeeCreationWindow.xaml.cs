using System;
using Pantheon.ViewModels.PopUp.Employees;
using Styx;
using Uranus;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for EmployeeCreationWindow.xaml
/// </summary>
public partial class EmployeeCreationWindow
{
    public EmployeeCreationVM? VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public EmployeeCreationWindow(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        InitializeComponent();
    }

    private async void EmployeeCreationWindow_OnInitialized(object? sender, EventArgs e)
    {
        VM = await EmployeeCreationVM.CreateAsync(Helios, Charon);
        DataContext = VM;
    }
}