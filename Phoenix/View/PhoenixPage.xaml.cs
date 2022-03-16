using Uranus.Staff;
using System;
using Uranus.Interfaces;

namespace Phoenix.View;

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