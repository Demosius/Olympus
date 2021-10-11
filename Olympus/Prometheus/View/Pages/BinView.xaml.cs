﻿using Olympus.Prometheus.ViewModel.Helpers;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Olympus.Prometheus.View.Pages
{
    /// <summary>
    /// Interaction logic for BinView.xaml
    /// </summary>
    public partial class BinView : BREADBase
    {
        public BinView()
        {
            InitializeComponent();
        }

        public override EDataType DataType => EDataType.NAVBin;
    }
}
