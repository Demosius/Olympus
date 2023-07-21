using System.Threading.Tasks;
using Deimos.ViewModels.Controls;
using Morpheus.ViewModels.Interfaces;

namespace Deimos.Views.Controls;

/// <summary>
/// Interaction logic for QAOperatorStatsView.xaml
/// </summary>
public partial class QAOperatorStatsView : IRefreshingControl
{
    public QAOperatorStatsVM VM { get; set; }

    public QAOperatorStatsView(QAToolVM qaToolVM)
    {
        VM = new QAOperatorStatsVM(qaToolVM);
        InitializeComponent();
        DataContext = VM;
    }

    public async Task RefreshDataAsync() => await VM.RefreshDataAsync();
}