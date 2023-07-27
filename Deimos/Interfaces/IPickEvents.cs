using System.Threading.Tasks;
using Deimos.ViewModels.Commands;

namespace Deimos.Interfaces;

public interface IPickEvents
{
    public UploadPickEventsCommand UploadPickEventsCommand { get; set; }

    public Task UploadPickEvents();
}