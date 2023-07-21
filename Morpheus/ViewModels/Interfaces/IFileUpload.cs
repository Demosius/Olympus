using System.Threading.Tasks;
using Morpheus.ViewModels.Commands;

namespace Morpheus.ViewModels.Interfaces;

public interface IFileUpload
{
    string DataFile { get; set; }

    public DataFromFileCommand DataFromFileCommand { get; set; }
    public SetDataFileCommand SetDataFileCommand { get; set; }

    public Task DataFromFileAsync();
    public void SetDataFile();
}