using System;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Khaos.Views;

/// <summary>
/// Interaction logic for KhaosPage.xaml
/// </summary>
public partial class KhaosPage : IProject
{
    public KhaosPage()
    {
        InitializeComponent();
    }

    public EProject Project => EProject.Khaos;

    public static bool RequiresUser => false;

    public void RefreshData()
    {
        throw new NotImplementedException();
    }
}