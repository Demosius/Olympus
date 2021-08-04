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

namespace Olympus.ViewModel.Components
{
    public class DBSelectionVM : INotifyPropertyChanged
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

        public OlympusVM OlympusVM { get; set; }

        public DBSelectionVM()
        {
            ChangeDatabaseCommand = new ChangeDatabaseCommand(this);
            MoveDatabaseCommand = new MoveDatabaseCommand(this);
            CopyDatabaseCommand = new CopyDatabaseCommand(this);
            DBString = App.Settings.SolLocation;
        }

        public DBSelectionVM(OlympusVM olympusVM) : this()
        {
            OlympusVM = olympusVM;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string SelectFolder()
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            folderBrowserDialog.ShowDialog();
            return SetSol(folderBrowserDialog.SelectedPath);
        }

        // Given a chosen path, makes sure that either it ends in Sol,
        // or adds a Sol folder in the location and returns that.
        private string SetSol(string path)
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
            // This in turn resets the chariots for both helios and charon.
            App.Settings.SetNewSolLocation(path);
            // Set DBString.
            DBString = path;
            OlympusVM.UserHandlerVM.CheckUser();
            OlympusVM.InventoryUpdaterVM.GetUpdateTimes();
        }

        public void ChangeDatabase()
        {
            string path;
            MessageBoxResult result = MessageBox.Show("Use Local database?", "DB Choice", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                path = App.BaseDirectory();
            else if (result == MessageBoxResult.Cancel)
                return;
            else
            {
                path = SelectFolder();
                // Empty string means cancelation.
                if (path == "") return;
            }
            // Make sure directory exists.
            if (!(Directory.Exists(path)))
                Directory.CreateDirectory(path);
            SetDatabase(path);
        }

        public void CopyDatabase()
        {
            string path = SelectFolder();
            // Empty string means cancelation.
            if (path == "") return;
            if (IsSubDirectory(App.Settings.SolLocation, path))
            {
                MessageBox.Show("Cannot copy to a subfolder of the current database.", 
                    "Failed Database Copy",
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
                return;
            }
            // Get a copy of existing database into chosen path.
            DirectoryCopy(App.Settings.SolLocation, path);

            SetDatabase(path);
        }

        public void MoveDatabase()
        {
            string path = SelectFolder();
            // Empty string means cancelation.
            if (path == "") return;
            if (IsSubDirectory(App.Settings.SolLocation, path))
            {
                MessageBox.Show("Cannot move to a subfolder of the current database.", 
                    "Failed Database Move", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
                return;
            }
            // Copy existing DB across to new location, and remove from old.
            DirectoryCopy(App.Settings.SolLocation, path);
            string oldPath = App.Settings.SolLocation;
            SetDatabase(path);
            Directory.Delete(oldPath, true);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
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
            DirectoryInfo parent = new DirectoryInfo(potentialParentDir);
            DirectoryInfo child = new DirectoryInfo(potentialChildDir);
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
