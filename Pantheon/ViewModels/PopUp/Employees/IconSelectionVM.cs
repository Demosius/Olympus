using Microsoft.Win32;
using Pantheon.ViewModels.Commands.Employees;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.Interfaces;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.PopUp.Employees;

public class IconSelectionVM : INotifyPropertyChanged, IImageSelector
{
    public EmployeeVM ParentVM { get; set; }
    public Helios Helios { get; set; }

    public Image? SelectedImage => SelectedIcon;

    #region Notifiable Properties

    private ObservableCollection<EmployeeIcon> icons;
    public ObservableCollection<EmployeeIcon> Icons
    {
        get => icons;
        set
        {
            icons = value;
            OnPropertyChanged(nameof(Icons));
        }
    }

    private EmployeeIcon? selectedIcon;
    public EmployeeIcon? SelectedIcon
    {
        get => selectedIcon;
        set
        {
            selectedIcon = value;
            OnPropertyChanged(nameof(SelectedIcon));
            IconName = selectedIcon?.Name ?? "";
        }
    }

    private string iconName;
    public string IconName
    {
        get => iconName;
        set
        {
            iconName = value;
            OnPropertyChanged(nameof(IconName));
            CheckCanSave();
        }
    }

    private bool canSaveImage;
    public bool CanSaveImage
    {
        get => canSaveImage;
        set
        {
            canSaveImage = value;
            OnPropertyChanged(nameof(CanSaveImage));
        }
    }

    #endregion

    public ConfirmImageSelectionCommand ConfirmImageSelectionCommand { get; }
    public SaveImageChangesCommand SaveImageChangesCommand { get; }
    public FindNewImageCommand FindNewImageCommand { get; set; }

    public IconSelectionVM(EmployeeVM parentVM)
    {
        ParentVM = parentVM;
        Helios = parentVM.Helios;

        icons = new ObservableCollection<EmployeeIcon>(AsyncHelper.RunSync(() => Helios.StaffReader.EmployeeIconsAsync()));

        iconName = string.Empty;

        ConfirmImageSelectionCommand = new ConfirmImageSelectionCommand(this);
        SaveImageChangesCommand = new SaveImageChangesCommand(this);
        FindNewImageCommand = new FindNewImageCommand(this);
    }

    private void CheckCanSave()
    {
        CanSaveImage = SelectedIcon is not null &&
                       SelectedIcon.Name != IconName &&
                       !SelectedIcon.Employees.Any() &&
                       Icons.All(i => i.Name != IconName);
    }

    public async Task SaveImageChangesAsync()
    {
        if (SelectedIcon is null) return;

        var icon = SelectedIcon;

        await Task.Run(() => Helios.StaffUpdater.RenameEmployeeIcon(ref icon, IconName));

        SelectedIcon = icon;
    }

    public void ConfirmImageSelection()
    {
        ParentVM.Icon = SelectedIcon;
    }

    public async Task FindNewImageAsync()
    {
        var dialog = new OpenFileDialog
        {
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            Filter = "image files (*.jpg,*.png,*.ico)|*.jpg;*.png;*.ico|All files (*.*)|*.*",
            FilterIndex = 0,
            Multiselect = false,
            Title = "Select Image File"
        };

        if (dialog.ShowDialog() != true) return;

        var icon = await Helios.StaffCreator.CreateEmployeeIconFromSourceFileAsync(dialog.FileName);

        if (icon is not null)
        {
            Icons.Add(icon);
        }
        OnPropertyChanged(nameof(EmployeeIcon.FullPath));

        SelectedIcon = icon;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}