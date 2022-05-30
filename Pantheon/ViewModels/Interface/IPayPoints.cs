using Pantheon.ViewModels.Commands.Employees;
using Styx;

namespace Pantheon.ViewModels.Interface;

public interface IPayPoints
{
    public Charon? Charon { get; set; }

    public AddPayPointCommand AddPayPointCommand { get; }

    public void AddPayPoint();
}