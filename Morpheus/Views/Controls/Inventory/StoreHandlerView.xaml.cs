using System;
using Morpheus.ViewModels.Controls.Inventory;
using Uranus;

namespace Morpheus.Views.Controls.Inventory;

/// <summary>
/// Interaction logic for StoreHandlerView.xaml
/// </summary>
public partial class StoreHandlerView
{
    public StoreHandlerVM VM { get; set; }
    public Helios Helios { get; set; }

    public StoreHandlerView(Helios helios)
    {
        Helios = helios;
        VM = StoreHandlerVM.CreateEmpty(helios);
        InitializeComponent();
        DataContext = VM;
    }

    private async void StoreHandlerView_OnInitialized(object? sender, EventArgs e)
    {
        VM = await StoreHandlerVM.CreateAsync(Helios);
        DataContext = VM;
    }
}