﻿using System;
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
using Olympus.Charon;

namespace Olympus.Components
{
    /// <summary>
    /// Interaction logic for SignIn.xaml
    /// </summary>
    public partial class UserHandler : UserControl
    {
        public Boatman Charon { get; set; }

        public UserHandler()
        {
            InitializeComponent();
            Charon = new Boatman();
        }
         
        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            LogIn signInForm = new LogIn();
            signInForm.ShowDialog();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            Register regForm = new Register();
            regForm.ShowDialog();
        }
    }
}