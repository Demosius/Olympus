using SQLite;
using System;
using System.IO;

namespace Uranus.Staff.Model;

public class Image
{
    [PrimaryKey] public string Name { get; set; }
    public string FileName { get; set; }

    private string fullPath;
    [Ignore] public string FullPath 
    {
        get
        {
            if (fullPath is null or "")
                fullPath = GetImageFilePath();
            return fullPath;
        }
        set => fullPath = value;
    }

    public virtual string GetImageFilePath()
    {
        // Current Directory
        var checkDir = Path.Combine(Directory.GetCurrentDirectory(), FileName);
        if (CheckPath(checkDir)) return checkDir;

        // User Image Directory.
        checkDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), FileName);
        if (CheckPath(checkDir)) return checkDir;

        // User Directory.
        checkDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), FileName);

        return CheckPath(checkDir) ? checkDir : "";
    }

    public virtual string GetImageFilePath(StaffReader reader)
    {
        // Check multiple locations for the image.

        var checkDir = GetImageFilePath();
        if (CheckPath(checkDir)) return checkDir;

        // Database directory.
        checkDir = Path.Combine(reader.BaseDirectory, FileName);
        if (CheckPath(checkDir)) return checkDir;

        // Staff EmployeeIcon Directory.
        checkDir = Path.Combine(reader.EmployeeIconDirectory, FileName);
        if (CheckPath(checkDir)) return checkDir;

        // Staff ProjectIcon Directory.
        checkDir = Path.Combine(reader.ProjectIconDirectory, FileName);
            
        return CheckPath(checkDir) ? checkDir : FileName;
    }

    // Given a full directory path to a file, checks if file exists.
    // If it does, set full path value to match, and return true.
    private bool CheckPath(string path)
    {
        if (!File.Exists(path)) return false;
        FullPath = path;
        return true;
    }
}