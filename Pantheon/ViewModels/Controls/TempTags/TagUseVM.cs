using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Pantheon.Annotations;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.TempTags;

public class TagUseVM : INotifyPropertyChanged
{
    public TagUse TagUse { get; set; }
    public TempTagVM TempTag { get; set; }

    #region TagUse Access

    public int ID => TagUse.ID;

    public int EmployeeID => TagUse.EmployeeID;

    public Employee? Employee => TagUse.Employee;

    public string EmployeeName => Employee?.FullName ?? (EmployeeID > 0 ? EmployeeID.ToString() : "(unknown employee)");

    public string TempTagRF_ID => TagUse.TempTagRF_ID;

    public DateTime TrueStart => TagUse.StartDate;
    public DateTime? TrueEnd => TagUse.EndDate;

    public string StartDate 
    { 
        get => TagUse.StartDate.ToString("dd-MMM-yyyy");
        set
        {
            if (!DateTime.TryParse(value, out var date)) return;
            
            TagUse.StartDate = date;
            OnPropertyChanged();
        }
    }

    public string EndDate
    {
        get => TagUse.EndDate?.ToString("dd-MMM-yyyy") ?? string.Empty;
        set
        {
            // Make sure that the user has appropriate permissions.
            if (!TempTag.ParentVM?.CanCreate ?? false) return;

            if (DateTime.TryParse(value, out var date))
            {
                TagUse.EndDate = date;
                TempTag.UpdateUse(this);
            }
            else
            {
                if (TagUse.EndDate is null || Employee is null) return;
                if (MessageBox.Show("Would you like to set this end date as null?\n\n" +
                                    $"This will re-assign the tag '{TempTagRF_ID}' to '{Employee}', clearing all use of this tag (including other users) from {StartDate} that might exist.",
                        "Clear End Date", MessageBoxButton.YesNo, MessageBoxImage.Warning) !=
                    MessageBoxResult.Yes) return;
                TagUse.EndDate = null;
                
                TempTag.SetEmployee(Employee);
                // ClearUseAfter includes database update. No need to do it explicitly.
                var removed = TempTag.ClearUseAfter(this);
                if (removed > 0)
                {
                    MessageBox.Show($"{removed} instances of historic use have been removed.", "Removed",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    TempTag.RefreshLayout();
                }
            }
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsActive));
        }
    }

    public bool IsActive => TagUse.EndDate is not null;

    #endregion

    public TagUseVM(TagUse tagUse, TempTagVM tempTagVM)
    {
        TagUse = tagUse;
        TempTag = tempTagVM;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}