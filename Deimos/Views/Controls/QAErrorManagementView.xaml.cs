using System.Threading.Tasks;
using Deimos.ViewModels.Controls;
using Morpheus.ViewModels.Controls;
using Morpheus.ViewModels.Interfaces;
using Uranus;

namespace Deimos.Views.Controls;

/// <summary>
/// Interaction logic for QAErrorManagementView.xaml
/// </summary>
public partial class QAErrorManagementView : IRefreshingControl
{
    public QAErrorManagementVM VM { get; set; }

    public QAErrorManagementView(Helios helios, ProgressBarVM progressBar)
    {
        VM = new QAErrorManagementVM(helios, progressBar);
        InitializeComponent();
        DataContext = VM;
    }

    public async Task RefreshDataAsync() => await VM.RefreshDataAsync();
}