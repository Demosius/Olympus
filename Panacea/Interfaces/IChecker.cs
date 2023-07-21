using System.Threading.Tasks;
using Panacea.ViewModels.Commands;

namespace Panacea.Interfaces;

public interface IChecker
{
    public RunChecksCommand RunChecksCommand { get; set; }
    public Task RunChecksAsync();
}