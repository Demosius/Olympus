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

public class AvatarSelectionVM : INotifyPropertyChanged, IImageSelector
{
    public EmployeeVM ParentVM { get; set; }
    public Helios Helios { get; set; }

    public Image? SelectedImage => SelectedAvatar;

    #region Notifiable Properties

    private ObservableCollection<EmployeeAvatar> avatars;
    public ObservableCollection<EmployeeAvatar> Avatars
    {
        get => avatars;
        set
        {
            avatars = value;
            OnPropertyChanged(nameof(Avatars));
        }
    }

    private EmployeeAvatar? selectedAvatar;
    public EmployeeAvatar? SelectedAvatar
    {
        get => selectedAvatar;
        set
        {
            selectedAvatar = value;
            OnPropertyChanged(nameof(SelectedAvatar));
            AvatarName = selectedAvatar?.Name ?? "";
        }
    }

    private string avatarName;
    public string AvatarName
    {
        get => avatarName;
        set
        {
            avatarName = value;
            OnPropertyChanged(nameof(AvatarName));
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

    private AvatarSelectionVM(EmployeeVM parentVM)
    {
        ParentVM = parentVM;
        Helios = ParentVM.Helios;

        avatars = new ObservableCollection<EmployeeAvatar>();

        avatarName = string.Empty;

        ConfirmImageSelectionCommand = new ConfirmImageSelectionCommand(this);
        SaveImageChangesCommand = new SaveImageChangesCommand(this);
        FindNewImageCommand = new FindNewImageCommand(this);
    }

    private async Task<AvatarSelectionVM> InitializeAsync()
    {
        var avatarTask = Helios.StaffReader.EmployeeAvatarsAsync();
        foreach (var employeeAvatar in await avatarTask)
            avatars.Add(employeeAvatar);

        return this;
    }

    public static Task<AvatarSelectionVM> CreateAsync(EmployeeVM parentVM)
    {
        var ret = new AvatarSelectionVM(parentVM);
        return ret.InitializeAsync();
    }

    private void CheckCanSave()
    {
        CanSaveImage = SelectedAvatar is not null &&
                       SelectedAvatar.Name != AvatarName &&
                       !SelectedAvatar.Employees.Any() &&
                       Avatars.All(i => i.Name != AvatarName);
    }
    
    public async Task SaveImageChangesAsync()
    {
        if (SelectedAvatar is null) return;

        var avatar = SelectedAvatar;

        await Task.Run(() => Helios.StaffUpdater.RenameEmployeeAvatar(avatar, AvatarName));

        SelectedAvatar = avatar;
    }

    public void ConfirmImageSelection()
    {
        ParentVM.Avatar = SelectedAvatar;
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

        var icon = await Helios.StaffCreator.CreateEmployeeAvatarFromSourceFileAsync(dialog.FileName);

        if (icon is not null) Avatars.Add(icon);

        SelectedAvatar = icon;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}