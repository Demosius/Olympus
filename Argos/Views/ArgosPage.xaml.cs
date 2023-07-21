using System;
using Argos.ViewModels;
using System.Threading.Tasks;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Argos.Views;

/// <summary>
/// Interaction logic for HadesPage.xaml
/// </summary>
public partial class ArgosPage : IProject
{
    public ArgosVM VM { get; set; }
    public Helios Helios { get; set; }

    public ArgosPage(Helios helios)
    {
        Helios = helios;
        InitializeComponent();
        VM = ArgosVM.CreateEmpty(helios);
        DataContext = VM;
    }

    private async void ArgosPage_OnInitialized(object? sender, EventArgs e)
    {
        VM = await ArgosVM.CreateAsync(Helios);
        DataContext = VM;
    }

    public EProject Project => EProject.Argos;

    public static bool RequiresUser => false;

    public async Task RefreshDataAsync()
    {
        await Task.Run(() => {});
    }
}