using Styx;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Uranus;
using Uranus.Annotations;
using Uranus.Users.Models;

namespace Hermes.ViewModels.Controls;

public class MessagingTabVM : INotifyPropertyChanged
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    private HermesDataSet DataSet;

    public MessagingTabVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        DataSet = Helios.UserReader.HermesDataSet(Charon.User ?? new User());
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}