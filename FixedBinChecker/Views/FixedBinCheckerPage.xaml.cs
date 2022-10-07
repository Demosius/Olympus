using System;
using FixedBinChecker.ViewModels;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace FixedBinChecker.Views;

/// <summary>
/// Interaction logic for FixedBinCheckerPage.xaml
/// </summary>
public partial class FixedBinCheckerPage : IProject
{
    public FixedBinCheckerPage(Helios helios)
    {
        InitializeComponent();
        DataContext = new FixedBinCheckerVM(helios);
    }

    public EProject Project => EProject.FixedBinChecker;

    public static bool RequiresUser => false;

    public void RefreshData()
    {
        throw new NotImplementedException();
    }
}