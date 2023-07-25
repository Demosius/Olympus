using System.ComponentModel;
using System.Runtime.CompilerServices;
using Morpheus.ViewModels.Commands;
using Morpheus.ViewModels.Interfaces;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Argos.ViewModels.PopUps;

public class FillProgressSelectionVM : INotifyPropertyChanged, IConfirm
{
    public string PromptString { get; set; }

    private EBatchFillProgress fillProgress;
    public EBatchFillProgress FillProgress
    {
        get => fillProgress;
        set
        {
            fillProgress = value;
            OnPropertyChanged();
        }
    }

    public ConfirmCommand ConfirmCommand { get; set; }
    public ConfirmAndCloseCommand ConfirmAndCloseCommand { get; set; }

    public FillProgressSelectionVM(string prompt)
    {
        PromptString = prompt;

        ConfirmCommand = new ConfirmCommand(this);
        ConfirmAndCloseCommand = new ConfirmAndCloseCommand(this);
    }

    public bool Confirm() => true;

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}