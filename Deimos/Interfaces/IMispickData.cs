using System.Threading.Tasks;
using Deimos.ViewModels.Commands;

namespace Deimos.Interfaces;

public interface IMispickData
{
    public UploadMispickDataCommand UploadMispickDataCommand { get; set; }
    
    public Task UploadMispickData();
}