using System;
using System.Windows;
using Pantheon.ViewModels.PopUp.Employees;
using Styx;
using Uranus;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for PayPointSelectionWindow.xaml
/// </summary>
public partial class PayPointSelectionWindow
{
    public PayPointSelectionVM? VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public PayPointSelectionWindow(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        InitializeComponent();
    }

    private async void PayPointSelectionWindow_OnInitialized(object? sender, EventArgs e)
    {
        VM = await PayPointSelectionVM.CreateAsync(Helios, Charon);
        DataContext = VM;
    }
}