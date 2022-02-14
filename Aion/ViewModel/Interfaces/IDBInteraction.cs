using Aion.ViewModel.Commands;

namespace Aion.ViewModel.Interfaces
{
    public interface IDBInteraction
    {
        public RefreshDataCommand RefreshDataCommand { get; set; }

        public void RefreshData();
        public void RepairData();
    }
}
