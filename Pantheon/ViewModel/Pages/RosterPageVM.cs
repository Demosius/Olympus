using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pantheon.Properties;
using Styx;
using Uranus;

namespace Pantheon.ViewModel.Pages;

internal class RosterPageVM : INotifyPropertyChanged
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    #region NotifiableProperties
        
    private DateTime minDate;
    public DateTime MinDate
    {
        get => minDate;
        set
        {
            minDate = value;
            OnPropertyChanged(nameof(MinDate));
        }
    }

    private DateTime maxDate;
    public DateTime MaxDate
    {
        get => maxDate;
        set
        {
            maxDate = value;
            OnPropertyChanged(nameof(MaxDate));
        }
    }

    #endregion

    #region Commands

        

    #endregion

    public RosterPageVM() { }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}