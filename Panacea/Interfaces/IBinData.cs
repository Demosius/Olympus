using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Panacea.ViewModels.Commands;

namespace Panacea.Interfaces;

public interface IBinData
{
    public BinsToClipboardCommand BinsToClipboardCommand { get; set; }

    public void BinsToClipboard();
}