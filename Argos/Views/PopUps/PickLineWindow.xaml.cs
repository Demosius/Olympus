using System.Collections.Generic;
using Argos.ViewModels.PopUps;
using Uranus.Inventory.Models;

namespace Argos.Views.PopUps;

/// <summary>
/// Interaction logic for PickLineWindow.xaml
/// </summary>
public partial class PickLineWindow
{
    public PickLinesVM VM { get; set; }

    public PickLineWindow(IEnumerable<PickLine> pickLines)
    {
        VM = new PickLinesVM(pickLines);
        InitializeComponent();
        DataContext = VM;
    }
}