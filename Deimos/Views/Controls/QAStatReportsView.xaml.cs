using System;
using System.Threading.Tasks;
using Deimos.ViewModels.Controls;
using Morpheus.ViewModels.Interfaces;

namespace Deimos.Views.Controls
{
    /// <summary>
    /// Interaction logic for QAStatReportsView.xaml
    /// </summary>
    public partial class QAStatReportsView : IRefreshingControl
    {
        public QAStatReportsVM VM { get; set; }
        public QAToolVM ParentVM { get; set; }

        public QAStatReportsView(QAToolVM qaToolVM)
        {
            ParentVM = qaToolVM;
            VM = QAStatReportsVM.CreateEmpty(qaToolVM);
            InitializeComponent();
            DataContext = VM;
        }

        private async void QAStatReportsView_OnInitialized(object? sender, EventArgs e)
        {
            VM = await QAStatReportsVM.CreateAsync(ParentVM);
            DataContext = VM;
        }
        
        public async Task RefreshDataAsync() => await VM.RefreshDataAsync();
    }
}
