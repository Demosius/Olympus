using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Uranus.Staff.Model
{
    public class Image
    {
        [PrimaryKey]
        public string Name { get; set; }
        public string FileName { get; set; }

        protected string fullPath = null;
        [Ignore]
        public string FullPath
        {
            get
            {
                if (fullPath is null) GetImageFilePath();
                if (Path.GetFileName(fullPath) != FileName) GetImageFilePath();
                return fullPath;
            }
        }

        public virtual void GetImageFilePath()
        {
            string checkDir;
            // Check multiple locations for the image.
            // Current Directory
            checkDir = Path.Combine(Directory.GetCurrentDirectory(), FileName);
            if (CheckPath(checkDir)) return;

            // App Directory
            checkDir = Path.Combine(App.BaseDirectory(), FileName);
            if (CheckPath(checkDir)) return;

            // Database directory.
            checkDir = Path.Combine(App.Settings.SolLocation, FileName);
            if (CheckPath(checkDir)) return;

            // Staff Database.
            checkDir = App.Helios.StaffReader.BaseDirectory;
            if (CheckPath(checkDir)) return;

            // Staff EmployeeIcon Directory.
            checkDir = App.Helios.StaffReader.EmployeeIconDirectory;
            if (CheckPath(checkDir)) return;

            // Staff ProjectIcon Directory.
            checkDir = App.Helios.StaffReader.ProjectIconDirectory;
            if (CheckPath(checkDir)) return;

            // User Image Directory.
            checkDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), FileName);
            if (CheckPath(checkDir)) return;

            // User Directory.
            checkDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), FileName);
            if (CheckPath(checkDir)) return;

        }

        // Given a full directory path to a file, checks if file exists.
        // If it does, set fullpath value to match, and return true.
        private bool CheckPath(string path)
        {
            if (File.Exists(path))
            {
                fullPath = path;
                return true;
            }
            return false;
        }
    }
}
