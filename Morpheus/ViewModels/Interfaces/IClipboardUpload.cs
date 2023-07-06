using System.Threading.Tasks;
using Morpheus.ViewModels.Commands;

namespace Morpheus.ViewModels.Interfaces;

public interface IClipboardUpload
{
    public UploadDataCommand UploadDataCommand { get; set; }

    public Task UploadDataAsync();
}
