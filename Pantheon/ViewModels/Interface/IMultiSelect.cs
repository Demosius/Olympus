using Pantheon.ViewModels.Commands.Generic;

namespace Pantheon.ViewModels.Interface;

public interface IMultiSelect
{
    public SelectAllCommand SelectAllCommand { get; set; }
    public DeSelectCommand DeSelectCommand { get; set; }

    void SelectAll();
    void DeSelect();
}