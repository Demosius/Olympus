using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hydra.ViewModels.Commands;
using Uranus.Interfaces;

namespace Hydra.Interfaces;

public interface IItemFilters : IFilters
{
    public FilterItemsFromClipboardCommand FilterItemsFromClipboardCommand { get; set; }

    public void FilterItemsFromClipboard();
}