using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Argos.ViewModels.Commands;
using Argos.ViewModels.Components;
using Uranus.Interfaces;

namespace Argos.Interfaces;

public interface IBatchTOGroupHandler : IDBInteraction
{
    public List<BatchTOGroupVM> AllGroups { get; set; }
    public ObservableCollection<BatchTOGroupVM> Groups { get; set; }
    public ObservableCollection<BatchTOGroupVM> SelectedGroups { get; set; }
    public BatchTOGroupVM? SelectedGroup { get; set; }

    public ZoneSplitCommand ZoneSplitCommand { get; set; }
    public CartonSplitCommand CartonSplitCommand { get; set; }
    public BaySplitCommand BaySplitCommand { get; set; }
    public CountSplitCommand CountSplitCommand { get; set; }
    public MergeCommand MergeCommand { get; set; }
    public RecoverOriginalFileCommand RecoverOriginalFileCommand { get; set; }

    Task ZoneSplit();
    Task CartonSplit();
    Task BaySplit();
    Task CountSplit();
    Task Merge();
    Task RecoverOriginalFile();
}