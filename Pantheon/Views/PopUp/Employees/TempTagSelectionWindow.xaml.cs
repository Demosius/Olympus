using System;
using Pantheon.ViewModels.PopUp.TempTags;
using Styx;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for TempTagSelectionWindow.xaml
/// </summary>
public partial class TempTagSelectionWindow
{
    public TempTagSelectionVM? VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public TempTag? TempTag => VM?.SelectedTag?.TempTag;

    public TempTagSelectionWindow(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        InitializeComponent();
    }

    private async void TempTagSelectionWindow_OnInitialized(object? sender, EventArgs e)
    {
        VM = await TempTagSelectionVM.CreateAsync(Helios, Charon);
        DataContext = VM;
    }
}