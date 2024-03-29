﻿using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Models;

public class LicenceImage : Image
{
    [ForeignKey(typeof(Licence))] public int LicenceNumber { get; set; }

    [ManyToOne(nameof(LicenceNumber), nameof(Models.Licence.Images), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
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

    public LicenceImage(Image image)
    {
        Name = image.Name;
        FileName = image.FileName;
    }

    // public override string GetImageFilePath(StaffReader reader) => Path.Combine(reader.LicenceImageDirectory, FileName);
}