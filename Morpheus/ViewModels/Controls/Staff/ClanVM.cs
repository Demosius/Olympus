using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Styx;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Morpheus.ViewModels.Controls.Staff;

public class ClanVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }
    public Clan Clan { get; set; }

    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }

    #region Clan Access

    public string Name
    {
        get => Clan.Name;
        set => _ = UpdateNameAsync(value);
    }

    public string LeaderID
    {
        get => Clan.LeaderID.ToString();
        set
        {
            if (CanUpdate && int.TryParse(value, out var val))
            {
                Clan.LeaderID = val;
                _ = SetHeadAsync();
            }
            OnPropertyChanged();
            OnPropertyChanged(nameof(Leader));
            _ = SaveAsync();
        }
    }

    public string Leader => Clan.Leader?.FullName ?? "";

    public string DepartmentName
    {
        get => Clan.DepartmentName;
        set
        {
            if (CanUpdate)
                Clan.DepartmentName = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }
    
    #endregion

    #region INotifyPropertyChanged Members



    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    public ClanVM(Clan department, Helios helios, Charon charon)
    {
        Clan = department;
        Helios = helios;
        Charon = charon;

        CanCreate = Charon.CanCreateClan();
        CanUpdate = Charon.CanUpdateClan();
        CanDelete = Charon.CanDeleteClan();

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    private async Task UpdateNameAsync(string newName)
    {
        if (CanUpdate && Name != newName)
            await Helios.StaffUpdater.ClanNameChangeAsync(Clan, newName);
        OnPropertyChanged(nameof(Name));
    }

    public async Task SetHeadAsync()
    {
        Clan.Leader = LeaderID == string.Empty ? null : await Helios.StaffReader.EmployeeAsync(Clan.LeaderID);
        OnPropertyChanged(nameof(Leader));
    }

    public async Task RefreshDataAsync()
    {
        Clan = await Helios.StaffReader.ClanAsync(Clan.Name) ?? Clan;
    }

    public async Task SaveAsync()
    {
        if (CanUpdate) await Helios.StaffUpdater.ClanAsync(Clan);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}