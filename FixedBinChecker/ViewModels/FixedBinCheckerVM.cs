using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using FixedBinChecker.ViewModels.Commands;
using Uranus;
using Uranus.Annotations;

namespace FixedBinChecker.ViewModels;

public class FixedBinCheckerVM : INotifyPropertyChanged
{
    public Helios Helios { get; set; }

    #region INotifyPropertyChanged Members

    private string fromZoneString;
    public string FromZoneString
    {
        get => fromZoneString;
        set
        {
            fromZoneString = value;
            OnPropertyChanged();
        }
    }

    private string fixedZoneString;
    public string FixedZoneString
    {
        get => fixedZoneString;
        set
        {
            fixedZoneString = value;
            OnPropertyChanged();
        }
    }


    private bool checkCase;
    public bool CheckCase
    {
        get => checkCase;
        set
        {
            checkCase = value;
            OnPropertyChanged();
        }
    }


    private bool checkPack;
    public bool CheckPack
    {
        get => checkPack;
        set
        {
            checkPack = value;
            OnPropertyChanged();
        }
    }


    private bool checkEach;
    public bool CheckEach
    {
        get => checkEach;
        set
        {
            checkEach = value;
            OnPropertyChanged();
        }
    }

    private bool checkExclusiveEach;
    public bool CheckExclusiveEach
    {
        get => checkExclusiveEach;
        set
        {
            checkExclusiveEach = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public RunChecksCommand RunChecksCommand { get; set; }

    #endregion

    public FixedBinCheckerVM(Helios helios)
    {
        Helios = helios;
        fromZoneString = string.Empty;
        fixedZoneString = string.Empty;

        RunChecksCommand = new RunChecksCommand(this);
    }

    public void RunChecks()
    {
        MessageBox.Show("Test");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}