using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cadmus.ViewModels.Commands;

namespace Cadmus.Interfaces;

/// <summary>
/// Denotes a view model that contains data-lines, whether they are for document lines or individual labels.
/// </summary>
public interface IDataLines
{
    public AddLineCommand AddLineCommand { get; set; }
    public void AddLine();
}