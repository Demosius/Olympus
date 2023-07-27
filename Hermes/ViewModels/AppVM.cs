using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Hermes.Views;
using Uranus.Annotations;

namespace Hermes.ViewModels;

public class AppVM : INotifyPropertyChanged
{
    public HermesPage HermesPage { get; set; }

    public AppVM()
    {
        HermesPage = new HermesPage(App.Helios, App.Charon);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}