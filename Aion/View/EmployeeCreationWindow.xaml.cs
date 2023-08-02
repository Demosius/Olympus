using System;
using Aion.ViewModels;
using Uranus;

namespace Aion.View;

public partial class EmployeeCreationWindow
{
    public EmployeeCreationVM? VM { get; set; }
    public Helios Helios { get; set; }

    public EmployeeCreationWindow(Helios helios)
    {
        Helios = helios;
        InitializeComponent();
    }

    private void EmployeeCreationWindow_OnInitialized(object? sender, EventArgs e)
    {
        VM = new EmployeeCreationVM(Helios);
        DataContext = VM;
    }
}