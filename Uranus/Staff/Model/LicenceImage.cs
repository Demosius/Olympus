﻿using SQLiteNetExtensions.Attributes;
using System.IO;

namespace Uranus.Staff.Model;

public class LicenceImage : Image
{
    [ForeignKey(typeof(Licence))] public int LicenceNumber { get; set; }

    [ManyToOne(nameof(LicenceNumber), nameof(Model.Licence.Images), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Licence? Licence { get; set; }

    public LicenceImage(int licenceNumber, Licence licence)
    {
        LicenceNumber = licenceNumber;
        Licence = licence;
    }

    public LicenceImage(string fullPath, string name, string fileName, int licenceNumber, Licence licence) : base(fullPath, name, fileName)
    {
        LicenceNumber = licenceNumber;
        Licence = licence;
    }

    public override string GetImageFilePath(StaffReader reader) => Path.Combine(reader.LicenceImageDirectory, FileName);
}