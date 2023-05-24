using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Uranus.Annotations;

namespace Morpheus.ViewModels.Controls;

public class ProgressTaskVM : INotifyPropertyChanged
{
    public string? Title { get; set; }

    public string? Action { get; set; }

    public int? Min { get; set; }

    public int? Max { get; set; }

    public int? Val { get; set; }

    public ProgressTaskVM(string? newTitle = null, string? newAction = null, int? min = null, int? max = null, int? val = null)
    {
        Title = newTitle;
        Action = newAction;
        Min = min;
        Max = max;
        Val = val;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}