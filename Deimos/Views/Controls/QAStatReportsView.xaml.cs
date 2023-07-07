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

        public QAStatReportsView(QAToolVM qaToolVM)
        {
            VM = new QAStatReportsVM(qaToolVM);
            InitializeComponent();
            DataContext = VM;
        }

        public async Task RefreshDataAsync() => await VM.RefreshDataAsync();
    }
}
