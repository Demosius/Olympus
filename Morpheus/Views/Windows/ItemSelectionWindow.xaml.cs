using System.Collections.Generic;
using Morpheus.ViewModels.Windows;
using Uranus.Inventory.Models;

namespace Morpheus.Views.Windows
{
    /// <summary>
    /// Interaction logic for ItemSelectionWindow.xaml
    /// </summary>
    public partial class ItemSelectionWindow
    {
        public NAVItem? Item => ((ItemSelectionVM) DataContext).SelectedItem;

        public ItemSelectionWindow(List<NAVItem> items)
        {
            InitializeComponent();
            DataContext = new ItemSelectionVM(items);
        }
    }
}
