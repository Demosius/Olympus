using Pantheon.ViewModel.Commands;
using Pantheon.ViewModel.Commands.Employees;
using Styx;

namespace Pantheon.ViewModel.Interface;

public interface IPayPoints
{
    public Charon? Charon { get; set; }

    public AddPayPointCommand AddPayPointCommand { get; }

    public void AddPayPoint();
}