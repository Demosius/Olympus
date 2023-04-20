using Morpheus.ViewModels.Commands;
using Uranus.Interfaces;

namespace Morpheus.ViewModels.Interfaces;

public interface IMultiSelect : IFilters
{
    public SelectAllCommand SelectAllCommand { get; set; }
    public DeselectAllCommand DeselectAllCommand { get; set; }
    public SelectFilteredCommand SelectFilteredCommand { get; set; }
    public DeselectFilteredCommand DeselectFilteredCommand { get; set; }
    public SelectFilteredExclusiveCommand SelectFilteredExclusiveCommand { get; set; }

    void SelectAll();
    void DeselectAll();
    void SelectFiltered();
    void DeselectFiltered();
    void SelectFilteredExclusive();
}