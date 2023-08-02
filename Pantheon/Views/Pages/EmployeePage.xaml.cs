using System;
using Pantheon.ViewModels.Pages;
using Styx;
using Uranus;

namespace Pantheon.Views.Pages;

/// <summary>
/// Interaction logic for EmployeePage.xaml
/// </summary>
public partial class EmployeePage
{
    public EmployeePageVM? VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public EmployeePage(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        InitializeComponent();
    }

    private async void EmployeePage_OnInitialized(object? sender, EventArgs e)
    {
        VM = await EmployeePageVM.CreateAsync(Helios, Charon);
        DataContext = VM;
    }
}