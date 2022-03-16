using Uranus.Commands;

namespace Uranus.Interfaces;

public interface IFilters
{
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }
    public void ClearFilters();
    public void ApplyFilters();
    public void ApplySorting();
}