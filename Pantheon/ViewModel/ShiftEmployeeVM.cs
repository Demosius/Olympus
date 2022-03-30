using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Pantheon.Annotations;
using Styx;
using Uranus;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel
{
    internal class ShiftEmployeeVM : INotifyPropertyChanged
    {
        public void SetData(Helios helios, Charon charon, Shift shift)
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
}
