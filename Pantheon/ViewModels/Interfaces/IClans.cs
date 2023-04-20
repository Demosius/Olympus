using Pantheon.ViewModels.Commands.Employees;
using Styx;

namespace Pantheon.ViewModels.Interfaces;

public interface IClans
{
    public Charon Charon { get; set; }

    public SelectClanCommand SelectClanCommand { get; set; }
    public ClearClanCommand ClearClanCommand { get; set; }

    public void SelectClan();
    public void ClearClan();
}