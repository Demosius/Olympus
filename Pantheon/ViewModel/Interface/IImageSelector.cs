using Pantheon.ViewModel.Commands;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Interface;

internal interface IImageSelector
{
    public ConfirmImageSelectionCommand ConfirmImageSelectionCommand { get; }
    public SaveImageChangesCommand SaveImageChangesCommand { get; }
    public FindNewImageCommand FindNewImageCommand { get; set; }
    public void ConfirmImageSelection();
    public void SaveImageChanges();
    public void FindNewImage();
    public bool CanSaveImage { get; set; }
    public Image? SelectedImage { get; }
}