﻿using System.Threading.Tasks;
using Morpheus.ViewModels.Interfaces;

namespace Quest.Views.Components
{
    /// <summary>
    /// Interaction logic for MappingView.xaml
    /// </summary>
    public partial class MappingView : IRefreshingControl
    {
        public MappingView()
        {
            InitializeComponent();
        }

        public Task RefreshDataAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
