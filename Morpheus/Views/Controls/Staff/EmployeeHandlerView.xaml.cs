using System;
using Morpheus.ViewModels.Controls.Staff;
using Styx;
using Uranus;

namespace Morpheus.Views.Controls.Staff;

/// <summary>
/// Interaction logic for EmployeeHandlerView.xaml
/// </summary>
public partial class EmployeeHandlerView
{
    public EmployeeHandlerVM VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public EmployeeHandlerView(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        VM = EmployeeHandlerVM.CreateEmpty(helios, charon);
        InitializeComponent();
        DataContext = VM;
    }


    private async void EmployeeHandlerView_OnInitialized(object? sender, EventArgs e)
    {
        VM = await EmployeeHandlerVM.CreateAsync(Helios, Charon);
        DataContext = VM;
    }
}