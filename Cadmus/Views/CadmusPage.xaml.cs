using System;
using Cadmus.ViewModels;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Cadmus.Views;

/// <summary>
/// Interaction logic for CadmusPage.xaml
/// </summary>
public partial class CadmusPage : IProject
{
    public CadmusPage()
    {
        InitializeComponent();
        DataContext = new CadmusVM(App.Helios);
    }

    public EProject Project => EProject.Cadmus;

    public static bool RequiresUser => false;

    public void RefreshData()
    {
        throw new NotImplementedException();
    }
}