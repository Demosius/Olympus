using System;
using Aion.ViewModels;
using Uranus;
using Uranus.Staff.Models;

namespace Aion.View;

/// <summary>
/// Interaction logic for EditEmployeeView.xaml
/// </summary>
public partial class EmployeeEditorWindow
{
    public EmployeeEditorVM? VM { get; set; }
    public Helios Helios { get; set; }
    public Employee Employee { get; set; }
    public bool IsNew { get; set; }

    public EmployeeEditorWindow(Helios helios, Employee employee, bool isNew)
    {
        Helios = helios;
        Employee = employee;
        IsNew = isNew;
        InitializeComponent();
    }

    private async void EmployeeEditorWindow_OnInitialized(object? sender, EventArgs e)
    {
        VM = await EmployeeEditorVM.CreateAsync(Helios, Employee, IsNew);
        DataContext = VM;
    }
}