using System.Threading.Tasks;
using Pantheon.ViewModels.Commands.Employees;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Interfaces;

public interface IImageSelector
{
    public ConfirmImageSelectionCommand ConfirmImageSelectionCommand { get; }
    public SaveImageChangesCommand SaveImageChangesCommand { get; }
    public FindNewImageCommand FindNewImageCommand { get; set; }
    public void ConfirmImageSelection();
    public Task SaveImageChangesAsync();
    public Task FindNewImageAsync();
    public bool CanSaveImage { get; set; }
    public Image? SelectedImage { get; }
}