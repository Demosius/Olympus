using System;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Phoenix.Views;

/// <summary>
/// Interaction logic for TorchPage.xaml
/// </summary>
public partial class PhoenixPage : IProject
{
    public PhoenixPage()
    {
        InitializeComponent();
    }

    public EProject Project => EProject.Phoenix;

    public static bool RequiresUser => false;

    public void RefreshData()
    {
        throw new NotImplementedException();
    }
}