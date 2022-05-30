using SQLite;
using System.IO;

namespace Uranus.Staff.Models;

public class Image
{
    [PrimaryKey] public string Name { get; set; }
    public string FileName { get; set; }

    [Ignore] public string Directory { get; set; }
    [Ignore] public string FullPath { get; set; }

    public Image()
    {
        Directory = string.Empty;
        FullPath = string.Empty;
        Name = string.Empty;
        FileName = string.Empty;
    }

    public Image(string directory, string name, string fileName)
    {
        Name = name;
        FileName = fileName;
        Directory = directory;
        FullPath = Path.Combine(directory, FileName);
    }

    public void SetDirectory(string directory)
    {
        Directory = directory;
        FullPath = Path.Combine(directory, FileName);
    }

}