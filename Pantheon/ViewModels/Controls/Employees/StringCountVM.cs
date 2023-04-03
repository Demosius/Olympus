using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;

namespace Pantheon.ViewModels.Controls.Employees;

/// <summary>
/// Holds a string typically representing some abstract concept that can be broken down to its name.
/// Holds a count of the thing, to denote the thing's use.
///
/// Examples: PayPoint and Location.
/// </summary>
public class StringCountVM : INotifyPropertyChanged
{
    public string Name { get; set; }
    public int Count { get; set; }

    public StringCountVM(string name, int count)
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