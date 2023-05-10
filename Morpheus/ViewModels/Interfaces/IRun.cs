using System.Threading.Tasks;
using Morpheus.ViewModels.Commands;

namespace Morpheus.ViewModels.Interfaces;

public interface IRun
{
    public bool CanRun { get; }
    public RunCommand RunCommand { get; set; }
    public Task Run();
}