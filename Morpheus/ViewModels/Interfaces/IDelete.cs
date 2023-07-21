using System.Threading.Tasks;
using Morpheus.ViewModels.Commands;

namespace Morpheus.ViewModels.Interfaces;

public interface IDelete<T>
{
    public T? SelectedItem { get; set; }

    public DeleteSelectedItemCommand<T> DeleteSelectedItemCommand { get; set; }

    public Task DeleteSelectedItemAsync();
}