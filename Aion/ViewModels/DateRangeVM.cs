using Aion.ViewModels.Commands;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Aion.ViewModels;

public class DateRangeVM : INotifyPropertyChanged
{
    public ShiftEntryPageVM? ShiftEntryPageVM { get; set; }

    private DateTime minDate;
    public DateTime MinDate
    {
        get => minDate;
        set
        {
            minDate = value;
            if (minDate > maxDate)
            {
                maxDate = minDate;
                OnPropertyChanged(nameof(MaxDate));
            }
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
            if (maxDate < minDate)
            {
                minDate = maxDate;
                OnPropertyChanged(nameof(MinDate));
            }
            OnPropertyChanged(nameof(MaxDate));
        }
    }

    public DateTime InitialMin { get; set; }
    public DateTime InitialMax { get; set; }

    public SetDateRangeCommand SetDateRangeCommand { get; set; }

    public DateRangeVM()
    {
        SetDateRangeCommand = new SetDateRangeCommand(this);
    }

    public void SetEditorVM(ShiftEntryPageVM editorVM)
    {
        ShiftEntryPageVM = editorVM;
        SetDateValues(ShiftEntryPageVM.MinDate, ShiftEntryPageVM.MaxDate);
    }

    public void SetDateValues(DateTime min, DateTime max)
    {
        MinDate = min;
        InitialMin = min;
        MaxDate = max;
        InitialMax = max;
    }
    
    public async Task SetDateRange()
    {
        if (ShiftEntryPageVM is null) return;
        ShiftEntryPageVM.MinDate = MinDate;
        ShiftEntryPageVM.MaxDate = MaxDate;
        await ShiftEntryPageVM.RefreshDataAsync(true);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [Uranus.Annotations.NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}