using System;
using Morpheus.ViewModels.Controls;
using Pantheon.ViewModels.Pages;
using Styx;
using Uranus;

namespace Pantheon.Views.Pages;

/// <summary>
/// Interaction logic for EmployeePage.xaml
/// </summary>
public partial class EmployeePage
{
    public EmployeePageVM VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public EmployeePage(Helios helios, Charon charon, ProgressBarVM progressBar)
    {
        Helios = helios;
        Charon = charon;
        ProgressBar = progressBar;
        VM = EmployeePageVM.CreateEmpty(Helios, Charon, ProgressBar);
        InitializeComponent();
        DataContext = VM;
    }

    private async void EmployeePage_OnInitialized(object? sender, EventArgs e)
    {
        VM = await EmployeePageVM.CreateAsync(Helios, Charon, ProgressBar);
        DataContext = VM;
    }
}