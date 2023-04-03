using Pantheon.ViewModels.Commands.Employees;
using Styx;

namespace Pantheon.ViewModels.Interface;

public interface IPayPoints
{
    public Charon Charon { get; set; }

    public SelectPayPointCommand SelectPayPointCommand { get; }
    public ClearPayPointCommand ClearPayPointCommand { get; set; }

    public void SelectPayPoint();
    public void ClearPayPoint();
}