using Pantheon.ViewModels.Commands.Employees;
using Styx;

namespace Pantheon.ViewModels.Interfaces;

public interface ILocations
{
    public Charon Charon { get; set; }

    public SelectLocationCommand SelectLocationCommand { get; set; }
    public ClearLocationCommand ClearLocationCommand { get; set; }

    public void SelectLocation();
    public void ClearLocation();
}