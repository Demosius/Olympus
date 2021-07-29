using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Uranus.Staff.Model
{
    public class LicenceImage : Image
    {
        [ForeignKey(typeof(Licence))]
        public int LicenceNumber { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Licence Licence { get; set; }

        public override void GetImageFilePath() => Path.Combine(App.Helios.StaffReader.LicenceImageDirectory, FileName);

    }
}
