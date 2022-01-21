using Olympus.Properties;
using Olympus.ViewModel.Commands;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Olympus.ViewModel.Utility
{
    public class DBManager : INotifyPropertyChanged
    {
        private string dbString;
        public string DBString
        {
            get => dbString;
            set
            {
                if (value == App.BaseDirectory())
                    dbString = "Local";
                else
                    dbString = value;
                OnPropertyChanged(nameof(DBString));
            }
        }

        public ChangeDatabaseCommand ChangeDatabaseCommand { get; set; }
        public MoveDatabaseCommand MoveDatabaseCommand { get; set; }
        public CopyDatabaseCommand CopyDatabaseCommand { get; set; }
        public UseLocalDBCommand UseLocalDBCommand { get; set; }
        public NewDatabaseCommand NewDatabaseCommand { get; set; }
        public MergeDatabaseCommand MergeDatabaseCommand { get; set; }

        public OlympusVM OlympusVM { get; set; }

        public DBManager()
        {
            ChangeDatabaseCommand = new(this);
            MoveDatabaseCommand = new(this);
            CopyDatabaseCommand = new(this);
            UseLocalDBCommand = new(this);
            NewDatabaseCommand = new(this);
            MergeDatabaseCommand = new(this);

            DBString = Settings.Default.SolLocation;
        }

        public DBManager(OlympusVM olympusVM) : this()
        {
            OlympusVM = olympusVM;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static string SelectFolder()
        {
            VistaFolderBrowserDialog folderBrowserDialog = new();
            _ = folderBrowserDialog.ShowDialog();
            return SetSol(folderBrowserDialog.SelectedPath);
        }

        // Given a chosen path, makes sure that either it ends in Sol,
        // or adds a Sol folder in the location and returns that.
        private static string SetSol(string path)
        {
            // Empty string means cancelation.
            if (path == "") return "";
            // Make sure the last folder is 'Sol'
            if (Path.GetFileName(path) != "Sol")
                path = Path.Combine(path, "Sol");
            return path;
        }

        private void SetDatabase(string path)
        {
            // Set App settings. 
            Settings.Default.SolLocation = path;
            // Set DBString.
            DBString = path;
            OlympusVM.UserHandlerVM.CheckUser();
            OlympusVM.InventoryUpdaterVM.GetUpdateTimes();
            // This in turn resets the chariots for both helios and charon.
            OlympusVM.ResetDB();
        }

        internal void UseLocalDB()
        {
            SetDatabase(App.BaseDirectory());
        }

        public void ChangeDatabase()
        {
            // TODO: Validate selected folder as exisitng Sol Location.
            string path;

            path = GetExistingSol();

            // Empty string means cancelation or failure to find existing Sol DB.
            if (path == "") return;

            SetDatabase(path);
        }
        
        public void NewDatabase()
        {
            string path;

            path = SelectFolder();
            // Empty string means cancelation.
            if (path == "") return;

            // Make sure directory exists.
            if (!(Directory.Exists(path)))
                _ = Directory.CreateDirectory(path);
            SetDatabase(path);
        }

        public void CopyDatabase()
        {
            string path = SelectFolder();
            // Empty string means cancelation.
            if (path == "") return;
            if (IsSubDirectory(Settings.Default.SolLocation, path))
            {
                _ = MessageBox.Show("Cannot copy to a subfolder of the current database.",
                    "Failed Database Copy",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            // Get a copy of existing database into chosen path.
            DirectoryCopy(Settings.Default.SolLocation, path);

            SetDatabase(path);
        }

        public void MoveDatabase()
        {
            string path = SelectFolder();
            // Empty string means cancelation.
            if (path == "") return;
            if (IsSubDirectory(Settings.Default.SolLocation, path))
            {
                _ = MessageBox.Show("Cannot move to a subfolder of the current database.",
                    "Failed Database Move",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            // Copy existing DB across to new location, and remove from old.
            DirectoryCopy(Settings.Default.SolLocation, path);
            string oldPath = Settings.Default.SolLocation;
            SetDatabase(path);
            Directory.Delete(oldPath, true);
        }

        /// <summary>
        /// User selects an exisinting DB-Sol location to merge with the current location.
        /// </summary>
        public void MergeDatabase()
        {
            string path = GetExistingSol();

            if (path == "") return;
            // TODO: Finish merging logic.
        }

        /// <summary>
        /// Allows the user to browse for an existing DB-Sol location.
        /// </summary>
        /// <returns>String representing Directory Path to Sol if found, otherwise empty string.</returns>
        private string GetExistingSol()
        {
            string path;

            path = SelectFolder();
            SetSol(path);

            if (CheckSolExistance(path)) return "";

            return path;
        }

        /// <summary>
        /// Checks to see if a proper DB-Sol structure exists at the given directory location.
        /// </summary>
        /// <param name="dirPath">Directory location for potential Sol.</param>
        /// <returns>True if Sol exists, else false.</returns>
        private static bool CheckSolExistance(string dirPath)
        {
            if (!Directory.Exists(dirPath)) return false;
            string equipmentPath = Path.Join(dirPath, "Equipment", "Equipment.sqlite");
            string staffPath = Path.Join(dirPath, "Staff", "Staff.sqlite");
            string usersPath = Path.Join(dirPath, "Users", "Users.sqlite");
            string inventoryPath = Path.Join(dirPath, "Inventory", "Inventory.sqlite");
            return Directory.Exists(equipmentPath) &&
                Directory.Exists(staffPath) &&
                Directory.Exists(inventoryPath) &&
                Directory.Exists(usersPath); 
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            _ = Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                _ = file.CopyTo(tempPath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        private bool IsSubDirectory(string potentialParentDir, string potentialChildDir)
        {
            DirectoryInfo parent = new(potentialParentDir);
            DirectoryInfo child = new(potentialChildDir);
            return IsSubDirectory(parent, child);
        }

        private bool IsSubDirectory(DirectoryInfo potentialParentDir, DirectoryInfo potentialChildDir)
        {
            if (potentialParentDir == potentialChildDir)
                return true;    // If they are the same, return true - as it means the same for our purposes.
            DirectoryInfo parent = potentialChildDir.Parent;
            if (parent is null)
                return false;   // Once there is no parent, that means that it must be false.
            if (parent.FullName == potentialParentDir.FullName)
                return true;
            return IsSubDirectory(potentialParentDir, parent);
        }
    }
}
