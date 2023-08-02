using System.ComponentModel;
using System.Runtime.CompilerServices;
using Morpheus.ViewModels.Commands;
using Morpheus.ViewModels.Interfaces;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Argos.ViewModels.PopUps;

public class ProgressSelectionVM : INotifyPropertyChanged, IConfirm
{
    public string PromptString { get; set; }

    private EBatchProgress batchProgress;
    public EBatchProgress BatchProgress 
    {
        get => batchProgress;
        set
        {
            batchProgress = value;
            OnPropertyChanged();
        }
    }

    public ConfirmCommand ConfirmCommand { get; set; }
    public ConfirmAndCloseCommand ConfirmAndCloseCommand { get; set; }

    public ProgressSelectionVM(string prompt)
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