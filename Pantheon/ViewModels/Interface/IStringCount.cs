using Pantheon.ViewModels.Commands.Employees;

namespace Pantheon.ViewModels.Interface;

public interface IStringCount
{
    public AddNewStringCountCommand AddNewStringCountCommand { get; set; }
    public DeleteStringCountCommand DeleteStringCountCommand { get; set; }
    public ConfirmStringCountSelectionCommand ConfirmStringCountSelectionCommand { get; set; }

    public void AddNewPayPoint();
    public void DeletePayPoint();

    public bool CanConfirm { get; }
    public bool CanDelete { get; }
    public bool CanAdd { get; }
}