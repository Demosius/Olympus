using System;
using Morpheus.ViewModels.Controls.Staff;
using Styx;
using Uranus;

namespace Morpheus.Views.Controls.Staff;

/// <summary>
/// Interaction logic for ClanHandlerView.xaml
/// </summary>
public partial class ClanHandlerView
{
    public ClanHandlerVM VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public ClanHandlerView(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        VM = ClanHandlerVM.CreateEmpty(Helios, Charon);
        InitializeComponent();
        DataContext = VM;
    }

    private async void ClanHandlerView_OnInitialized(object? sender, EventArgs e)
    {
        VM = await ClanHandlerVM.CreateAsync(Helios, Charon);
        DataContext = VM;
    }
}