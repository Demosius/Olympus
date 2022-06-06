using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hydra.ViewModels.Controls;

namespace Hydra.Views.Controls
{
    /// <summary>
    /// Interaction logic for ItemLevelsView.xaml
    /// </summary>
    public partial class ItemLevelsView : UserControl
    {
        public ItemLevelsView()
        {
            InitializeComponent();
        }
        
        private void DataGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var addedItems = e.AddedCells;
            if (!addedItems.Any()) return;
            var c = addedItems[0].Column;
            ((ItemLevelsVM)DataContext).SelectedObject = ((DataRowView)addedItems[0].Item).Row.ItemArray[c.DisplayIndex];
        }

        private void DataGrid_OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var col = e.Column;

            if (col.Header.ToString() != "Item")
            {
                ((DataGridTextColumn) col).ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters =
                    {
                        new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center)
                    }
                };
            }
        }
    }
}
