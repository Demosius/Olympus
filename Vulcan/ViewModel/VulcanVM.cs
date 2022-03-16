using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Vulcan.Properties;

namespace Vulcan.ViewModel;

public class VulcanVM : INotifyPropertyChanged
{
    public void RefreshData()
    {
        throw new NotImplementedException();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}