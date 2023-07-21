using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Morpheus.ViewModels.Windows;
using Uranus.Inventory.Models;

namespace Morpheus.Views.Windows;

/// <summary>
/// Interaction logic for ItemSelectionWindow.xaml
/// </summary>
public partial class ItemSelectionWindow
{
    public ItemSelectionVM VM { get; set; }
    public NAVItem? Item => VM.SelectedItem;
    public List<NAVItem> Items => VM.SelectedItems;

    public ItemSelectionWindow(List<NAVItem> items, SelectionMode selectionMode = SelectionMode.Single)
    {
        VM = new ItemSelectionVM(items, selectionMode);
        InitializeComponent();
        DataContext = VM;
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (VM.SelectionMode == SelectionMode.Single) return;
        VM.SelectedItems = ItemGrid.SelectedItems
            .Cast<NAVItem>()
            .ToList();
    }
}