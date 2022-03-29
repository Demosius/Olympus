using Microsoft.Win32;
using Pantheon.Annotations;
using Pantheon.ViewModel.Commands;
using Pantheon.ViewModel.Interface;
using Pantheon.ViewModel.Pages;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Uranus;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel;

internal class IconSelectionVM : INotifyPropertyChanged, IImageSelector
{
    public EmployeePageVM? ParentVM { get; set; }
    public Helios? Helios { get; set; }

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

    public IconSelectionVM()
    {
        icons = new ObservableCollection<EmployeeIcon>();
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

    public void SetDataSource(EmployeePageVM employeePageVM)
    {
        ParentVM = employeePageVM;
        Helios = ParentVM.Helios;
        if (ParentVM?.EmployeeDataSet is not null)
            Icons = new ObservableCollection<EmployeeIcon>(ParentVM.EmployeeDataSet.EmployeeIcons.Values);
    }
    public void SaveImageChanges()
    {
        if (SelectedIcon is null || Helios is null) return;

        var icon = SelectedIcon;

        Helios.StaffUpdater.RenameEmployeeIcon(ref icon, IconName);

        SelectedIcon = icon;
    }

    public void ConfirmImageSelection()
    {
        if (ParentVM?.SelectedEmployee is null) return;
        ParentVM.SelectedEmployee.Icon = SelectedIcon;
    }

    public void FindNewImage()
    {
        if (Helios is null) return;

        var dialog = new OpenFileDialog
        {
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            Filter = "image files (*.jpg,*.png,*.ico)|*.jpg;*.png;*.ico|All files (*.*)|*.*",
            FilterIndex = 0,
            Multiselect = false,
            Title = "Select Image File"
        };

        if (dialog.ShowDialog() != true) return;

        var icon = Helios.StaffCreator.CreateEmployeeIconFromSourceFile(dialog.FileName);

        if (icon is not null)
        {
            Icons.Add(icon);
            ParentVM?.EmployeeDataSet?.EmployeeIcons.Add(icon.Name, icon);
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