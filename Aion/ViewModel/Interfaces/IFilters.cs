using Aion.ViewModel.Commands;

namespace Aion.ViewModel.Interfaces
{
    public interface IFilters
    {
        public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
        public ClearFiltersCommand ClearFiltersCommand { get; set; }
        public void ClearFilters();
        public void ApplyFilters();
    }
}
