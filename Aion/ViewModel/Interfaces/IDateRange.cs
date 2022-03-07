using Aion.ViewModel.Commands;

namespace Aion.ViewModel.Interfaces;

public interface IDateRange
{
    public LaunchDateRangeCommand LaunchDateRangeCommand { get; set; }
    public void LaunchDateRangeWindow();
}