using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Olympus;

namespace Olympus.Components
{
    /// <summary>
    /// Interaction logic for DB_Selection.xaml
    /// </summary>
    public partial class DatabaseSelection : UserControl
    {
        public string DBString { get; set; }

        public DatabaseSelection()
        {
            InitializeComponent();
            SetDisplay();
        }

        private void SetDisplay()
        {
            DBString = $"DB Location: {Toolbox.GetSol()}";
            DBDisplay.DataContext = DBString;
        }

    }
}
