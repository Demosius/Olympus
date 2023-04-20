using System.Collections.ObjectModel;
using Pantheon.ViewModels.Controls.Shifts;
using Styx;
using Uranus;

namespace Pantheon.ViewModels.Interfaces;

public interface IShiftManagerVM
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public ObservableCollection<ShiftVM> ShiftVMs { get; set; }

    public void DeleteShift(ShiftVM shift);
}