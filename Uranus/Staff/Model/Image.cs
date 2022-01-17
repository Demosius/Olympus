using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Staff.Model
{
    public class Image
    {
        [PrimaryKey]
        public string Name { get; set; }
        public string FileName { get; set; }

        [Ignore]
        public string FullPath { get; set; }


        public virtual string GetImageFilePath(StaffReader reader)
        {
            string checkDir;
            // Check multiple locations for the image.
            
            // Current Directory
            checkDir = Path.Combine(Directory.GetCurrentDirectory(), FileName);
            if (CheckPath(checkDir)) return checkDir;

            // Database directory.
            checkDir = Path.Combine(reader.BaseDirectory, FileName);
            if (CheckPath(checkDir)) return checkDir;

            // Staff EmployeeIcon Directory.
            checkDir = Path.Combine(reader.EmployeeIconDirectory, FileName);
            if (CheckPath(checkDir)) return checkDir;

            // Staff ProjectIcon Directory.
            checkDir = Path.Combine(reader.ProjectIconDirectory, FileName);
            if (CheckPath(checkDir)) return checkDir;

            // User Image Directory.
            checkDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), FileName);
            if (CheckPath(checkDir)) return checkDir;

            // User Directory.
            checkDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), FileName);
            if (CheckPath(checkDir)) return checkDir;

            return FileName;
        }

        // Given a full directory path to a file, checks if file exists.
        // If it does, set fullpath value to match, and return true.
        private bool CheckPath(string path)
        {
            if (File.Exists(path))
            {
                FullPath = path;
                return true;
            }
            return false;
        }
    }
}
