﻿using Uranus.Staff;
using System;
using System.Windows.Controls;
using Uranus;

namespace Phoenix.View
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

        public EProject EProject => EProject.Phoenix;

        public void RefreshData()
        {
            throw new NotImplementedException();
        }
    }
}
