using Hades.ViewModels;
using System;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Hades.Views;

/// <summary>
/// Interaction logic for HadesPage.xaml
/// </summary>
public partial class HadesPage : IProject
{
    public HadesPage(Helios helios)
    {
        InitializeComponent();
        DataContext = new HadesVM(helios);
    }

    public EProject Project => EProject.Hades;

    public static bool RequiresUser => false;

    public void RefreshData()
    {
        throw new NotImplementedException();
    }
}