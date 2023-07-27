using System.Threading.Tasks;

namespace Morpheus.ViewModels.Interfaces;

public interface IRefreshingControl
{
    public Task RefreshDataAsync();
}