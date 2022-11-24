using Sphynx.ViewModels;
using Styx;
using System;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Sphynx.Views;

/// <summary>
/// Interaction logic for SphynxPage.xaml
/// </summary>
public partial class SphynxPage : IProject
{
    public SphynxPage(Helios helios, Charon charon)
    {
        InitializeComponent();
        DataContext = new SphynxVM(helios, charon);
    }

    public EProject Project => EProject.Sphynx;

    public static bool RequiresUser => false;

    public void RefreshData()
    {
        throw new NotImplementedException();
    }
}