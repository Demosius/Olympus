using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Panacea.ViewModels.Commands;
using Uranus;
using Uranus.Annotations;

namespace Panacea.ViewModels.Components;

public class PotentialNegativeVM : INotifyPropertyChanged
{
    public Helios Helios { get; set; }


    #region Commands

    public RunPotentialNegativesCheckCommand RunPotentialNegativesCheckCommand { get; set; }

    #endregion

    public PotentialNegativeVM(Helios helios)
    {
        Helios = helios;
    }
    
    public void RunPotentialNegativesCheck()
    {
        throw new NotImplementedException();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}