using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Hydra.ViewModels;
using Hydra.ViewModels.Controls;

namespace Hydra.Views
{
    /// <summary>
    /// Interaction logic for ItemSelectionWindow.xaml
    /// </summary>
    public partial class ItemSelectionWindow
    {
        public ItemSelectionWindow(ItemSelectionVM vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
