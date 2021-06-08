using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Olympus.Helios.Staff;

namespace Olympus.Helios
{
    public class GetStaff
    {

        public static DataSet DataSet()
        {
            StaffChariot chariot = new StaffChariot(Toolbox.GetSol());
            return chariot.PullFullDataSet();
        }

    }

    public class PutStaff
    {

    }

    public class PostStaff
    {

    }

    public class DeleteStaff
    {

    }
}
