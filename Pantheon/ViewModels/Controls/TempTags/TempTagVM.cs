using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.TempTags;

public class TempTagVM : INotifyPropertyChanged
{
    public TempTag TempTag { get; }

    // May or may not be used as the Parent VM, depending on from where this object is created/managed.
    public TempTagManagementVM? ParentVM { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<TagUseVM> Usage { get; }

    #endregion

    #region TempTag Access

    public List<TagUse> TagUses => TempTag.TagUse;

    public bool IsAssigned => TempTag.Employee is not null;

    public string RF_ID => TempTag.RF_ID;

    public Employee? Employee => TempTag.Employee;

    public int EmployeeID => TempTag.EmployeeID;

    public string EmployeeString => Employee is null ? EmployeeID == 0 ? "" : EmployeeID.ToString() : $"{Employee.ID} - {Employee.FullName}";

    #endregion

    public TempTagVM(TempTag tempTag)
    {
        TempTag = tempTag;
        Usage = new ObservableCollection<TagUseVM>();
        foreach (var tagUse in TagUses.OrderBy(use => use.StartDate))
        {
            Usage.Add(new TagUseVM(tagUse, this));
        }
    }

    public TempTagVM(TempTag tempTag, TempTagManagementVM? parentVM) : this(tempTag)
    {
        ParentVM = parentVM;
    }

    public void SetEmployee(Employee employee, bool isNew = false)
    {
        if (Employee is not null)
        {
            Employee.TempTag = null;
            Employee.TempTagRF_ID = string.Empty;
        }
        TempTag.SetEmployee(employee, isNew);
        OnPropertyChanged(nameof(Employee));
        OnPropertyChanged(nameof(EmployeeString));
    }

    /// <summary>
    /// Execute OnPropertyChanged on members that would signify a change in Employee association.
    /// </summary>
    public void RefreshEmployee()
    {
        OnPropertyChanged(nameof(Employee));
        OnPropertyChanged(nameof(EmployeeString));
        OnPropertyChanged(nameof(IsAssigned));
        OnPropertyChanged(nameof(EmployeeID));
        // Also reset the use VMs
        RefreshUsage();
    }

    private void RefreshUsage()
    {
        Usage.Clear();
        foreach (var tagUse in TagUses.Select(u => new TagUseVM(u, this)).OrderBy(u => u.StartDate))
            Usage.Add(tagUse);
        ParentVM?.SetTagUse();
    }

    /// <summary>
    /// Clears reported use that comes after the start of the given usage.
    /// </summary>
    /// <param name="tagUse"></param>
    public int ClearUseAfter(TagUseVM tagUse)
    {
        if (!ReferenceEquals(tagUse.TempTag, this)) return 0;
        var count = TagUses.RemoveAll(use => use.StartDate >= tagUse.TrueStart && use.ID != tagUse.TagUse.ID);
        var tagUseVMs = Usage.Where(use => use.ID == tagUse.ID || use.TrueStart < tagUse.TrueStart).ToList();
        // Test for matching data between raw and VM
        if (tagUseVMs.Count != TagUses.Count)
            throw new DataException("Incongruent reduction of raw data compared to view model data.");

        Usage.Clear();
        foreach (var tagUseVM in tagUseVMs)
            Usage.Add(tagUseVM);

        // Update DB if possible.
        ParentVM?.UpdateTagUse(this);

        return count;
    }

    public void RefreshLayout() => ParentVM?.RefreshTagView();

    public int UpdateUse(TagUseVM tagUseVM) => ParentVM?.UpdateTagUse(tagUseVM) ?? 0;

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}