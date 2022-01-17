﻿using Olympus.Model;
using Uranus.Staff;
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

namespace Olympus.Torch.View
{
    /// <summary>
    /// Interaction logic for TorchPage.xaml
    /// </summary>
    public partial class TorchPage : Page, IProject
    {
        public TorchPage()
        {
            InitializeComponent();
        }

        public EProject EProject => EProject.Torch;
    }
}
