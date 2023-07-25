using System.Threading.Tasks;
using Morpheus.ViewModels.Commands;

namespace Morpheus.ViewModels.Interfaces;

public interface ICreate
{
    public CreateNewItemCommand CreateNewItemCommand { get; set; }

    public Task CreateNewItemAsync();
}