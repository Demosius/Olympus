using Uranus.Staff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectToolbox
{
    public interface IProject
    {
        EProject EProject { get; }
        public void RefreshData();
    }
}
