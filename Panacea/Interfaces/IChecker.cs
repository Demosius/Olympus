using Panacea.ViewModels.Commands;

namespace Panacea.Interfaces;

public interface IChecker
{
    public RunChecksCommand RunChecksCommand { get; set; }
    void RunChecks();
}