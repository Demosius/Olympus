using System.Threading.Tasks;
using Deimos.ViewModels.Commands;

namespace Deimos.Interfaces;

public interface IRecalculate
{
    public RecalculateCommand RecalculateCommand { get; set; }

    public Task Recalculate(object? parameter);
}