using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Panacea.ViewModels.Commands;

namespace Panacea.Interfaces;

public interface IItemData
{
    public ItemsToClipboardCommand ItemsToClipboardCommand { get; set; }

    public void ItemsToClipboard();
}