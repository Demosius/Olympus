using Argos.ViewModels;
using System;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Argos.Views;

/// <summary>
/// Interaction logic for HadesPage.xaml
/// </summary>
public partial class ArgosPage : IProject
{
    public ArgosPage(Helios helios)
    {
        InitializeComponent();
        DataContext = new ArgosVM(helios);
    }

    public EProject Project => EProject.Argos;

    public static bool RequiresUser => false;

    public void RefreshData()
    {
        throw new NotImplementedException();
    }
}