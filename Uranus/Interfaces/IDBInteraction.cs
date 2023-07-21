using System.Threading.Tasks;
using Uranus.Commands;

namespace Uranus.Interfaces;

public interface IDBInteraction
{
    public Helios Helios { get; set; }

    public RefreshDataCommand RefreshDataCommand { get; set; }

    public Task RefreshDataAsync();
}