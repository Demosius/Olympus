using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;
using Uranus.Staff.Models;

namespace Deimos.ViewModels.Controls;

public class QAOperatorVM : INotifyPropertyChanged
{
    public Employee Operator { get; set; }

    #region Operator Access



    #endregion

    #region INotifyPropertyChanged Members



    #endregion

    #region Commands

    

    #endregion

    public QAOperatorVM(Employee operator)
    {

    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}