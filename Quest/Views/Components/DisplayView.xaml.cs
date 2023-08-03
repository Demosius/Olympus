using System;
using System.Threading.Tasks;
using Morpheus.ViewModels.Interfaces;

namespace Quest.Views.Components
{
    /// <summary>
    /// Interaction logic for DisplayView.xaml
    /// </summary>
    public partial class DisplayView : IRefreshingControl
    {
        public DisplayView()
        {
            InitializeComponent();
        }

        public Task RefreshDataAsync()
        {
            throw new NotImplementedException();
        }
    }
}
