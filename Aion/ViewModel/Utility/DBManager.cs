using System.ComponentModel;
using System.IO;
using System.Windows;
using Aion.Properties;
using Aion.ViewModel.Commands;
using Ookii.Dialogs.Wpf;

namespace Aion.ViewModel.Utility
{
    public class DBManager : INotifyPropertyChanged
    {
        private string dbString;
        public string DBString
        {
            get => dbString;
            set
            {
                dbString = value == App.BaseDirectory() ? "Local" : value;
                OnPropertyChanged(nameof(DBString));
            }
        }

        public ChangeDatabaseCommand ChangeDatabaseCommand { get; set; }
        public MoveDatabaseCommand MoveDatabaseCommand { get; set; }
        public CopyDatabaseCommand CopyDatabaseCommand { get; set; }
        public UseLocalDBCommand UseLocalDBCommand { get; set; }
        public NewDatabaseCommand NewDatabaseCommand { get; set; }
        public MergeDatabaseCommand MergeDatabaseCommand { get; set; }

        public AppVM ParentVM { get; set; }

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

        public DBManager(AppVM parentVM) : this()
        {
            ParentVM = parentVM;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
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
            // Empty string means cancellation.
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
            // This in turn resets the chariots for both helios and charon.
            ParentVM.ResetDB();
        }

        internal void UseLocalDB()
        {
            SetDatabase(App.BaseDirectory());
        }

        public void ChangeDatabase()
        {
            // TODO: Validate selected folder as existing Sol Location.

            var path = GetExistingSol();

            // Empty string means cancellation or failure to find existing Sol DB.
            if (path == "") return;

            SetDatabase(path);
        }

        public void NewDatabase()
        {
            var path = SelectFolder();
            // Empty string means cancellation.
            if (path == "") return;

            // Make sure directory exists.
            if (!Directory.Exists(path))
                _ = Directory.CreateDirectory(path);
            SetDatabase(path);
        }

        public void CopyDatabase()
        {
            var path = SelectFolder();
            // Empty string means cancellation.
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
            var path = SelectFolder();
            // Empty string means cancellation.
            if (path == "") return;
            if (IsSubDirectory(Settings.Default.SolLocation, path))
            {
                _ = MessageBox.Show("Cannot move to a subfolder of the current database.",
                    "Failed Database PartialMove",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            // Copy existing DB across to new location, and remove from old.
            DirectoryCopy(Settings.Default.SolLocation, path);
            var oldPath = Settings.Default.SolLocation;
            SetDatabase(path);
            Directory.Delete(oldPath, true);
        }

        /// <summary>
        /// User selects an existing DB-Sol location to merge with the current location.
        /// </summary>
        public static void MergeDatabase()
        {
            var path = GetExistingSol();

            if (path == "" || path == Settings.Default.SolLocation) return;
            // TODO: Finish merging logic.
            {

            }
        }

        /// <summary>
        /// Allows the user to browse for an existing DB-Sol location.
        /// </summary>
        /// <returns>String representing Directory Path to Sol if found, otherwise empty string.</returns>
        private static string GetExistingSol()
        {
            var path = SelectFolder();
            SetSol(path);

            if (CheckSolExistence(path)) return "";

            return path;
        }

        /// <summary>
        /// Checks to see if a proper DB-Sol structure exists at the given directory location.
        /// </summary>
        /// <param name="dirPath">Directory location for potential Sol.</param>
        /// <returns>True if Sol exists, else false.</returns>
        private static bool CheckSolExistence(string dirPath)
        {
            if (!Directory.Exists(dirPath)) return false;
            var equipmentPath = Path.Join(dirPath, "Equipment", "Equipment.sqlite");
            var staffPath = Path.Join(dirPath, "Staff", "Staff.sqlite");
            var usersPath = Path.Join(dirPath, "Users", "Users.sqlite");
            var inventoryPath = Path.Join(dirPath, "Inventory", "Inventory.sqlite");
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

            var dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            _ = Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var tempPath = Path.Combine(destDirName, file.Name);
                _ = file.CopyTo(tempPath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (!copySubDirs) return;

            foreach (var subDir in dirs)
            {
                var tempPath = Path.Combine(destDirName, subDir.Name);
                DirectoryCopy(subDir.FullName, tempPath);
            }

        }

        private static bool IsSubDirectory(string potentialParentDir, string potentialChildDir)
        {
            DirectoryInfo parent = new(potentialParentDir);
            DirectoryInfo child = new(potentialChildDir);
            return IsSubDirectory(parent, child);
        }

        private static bool IsSubDirectory(FileSystemInfo potentialParentDir, DirectoryInfo potentialChildDir)
        {
            if (potentialParentDir == potentialChildDir)
                return true;    // If they are the same, return true - as it means the same for our purposes.
            var parent = potentialChildDir.Parent;
            if (parent is null)
                return false;   // Once there is no parent, that means that it must be false.
            return parent.FullName == potentialParentDir.FullName || IsSubDirectory(potentialParentDir, parent);
        }
    }
}
