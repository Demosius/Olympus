﻿using SQLiteNetExtensions.Attributes;
using System.IO;

namespace Uranus.Staff.Model
{
    public class LicenceImage : Image
    {
        [ForeignKey(typeof(Licence))]
        public int LicenceNumber { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Licence Licence { get; set; }

        public override string GetImageFilePath(StaffReader reader) => Path.Combine(reader.LicenceImageDirectory, FileName);

    }
}