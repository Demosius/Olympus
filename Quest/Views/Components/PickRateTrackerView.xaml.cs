using System;
using System.Threading.Tasks;
using Morpheus.ViewModels.Controls;
using Morpheus.ViewModels.Interfaces;
using Quest.ViewModels.Components;
using Uranus;

namespace Quest.Views.Components;

/// <summary>
/// Interaction logic for PickRateTrackerView.xaml
/// </summary>
public partial class PickRateTrackerView : IRefreshingControl
{
    public PickRateTrackerVM VM { get; set; }
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public PickRateTrackerView(Helios helios, ProgressBarVM progressBar)
    {
        Helios = helios;
        ProgressBar = progressBar;
        VM = PickRateTrackerVM.CreateEmpty(helios, progressBar);
        InitializeComponent();
        DataContext = VM;
    }

    private async void PickRateTrackerView_OnInitialized(object? sender, EventArgs e)
    {
        VM = await PickRateTrackerVM.CreateAsync(Helios, ProgressBar);
        DataContext = VM;
    }

    public async Task RefreshDataAsync()
    {
        await VM.RefreshDataAsync();
    }
}