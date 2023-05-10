using System.Threading.Tasks;
using Aion.ViewModels.Commands;

namespace Aion.ViewModels.Interfaces;

public interface IDateRange
{
    public LaunchDateRangeCommand LaunchDateRangeCommand { get; set; }
    public Task LaunchDateRangeWindowAsync();
}