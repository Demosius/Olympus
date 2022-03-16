using Uranus.Staff;
using System;
using Styx;
using Uranus;
using Uranus.Interfaces;

namespace Pantheon.View;

/// <summary>
/// Interaction logic for PantheonPage.xaml
/// </summary>
public partial class PantheonPage : IProject

{
    public PantheonPage(Charon charon, Helios helios)
    {
        InitializeComponent();
        VM.SetDataSources(charon, helios);
    }

    public EProject Project => EProject.Pantheon;

    public static bool RequiresUser => true;

    public void RefreshData()
    {
        throw new NotImplementedException();
    }
}