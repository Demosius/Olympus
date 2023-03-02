using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;

namespace Pantheon.ViewModels.Controls.Employees;

public class PayPointVM : INotifyPropertyChanged
{
    public string Name { get; set; }
    public int Count { get; set; }

    public PayPointVM(string name, int count)
    {
        Name = name;
        Count = count;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}