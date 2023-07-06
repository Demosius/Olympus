using Deimos.ViewModels.Controls;
using Morpheus.ViewModels.Controls;
using Uranus;

namespace Deimos.Views.Controls;

/// <summary>
/// Interaction logic for QAErrorManagementView.xaml
/// </summary>
public partial class QAErrorManagementView
{
    public QAErrorManagementVM VM { get; set; }

    public QAErrorManagementView(Helios helios, ProgressBarVM progressBar)
    {
        VM = new QAErrorManagementVM(helios, progressBar);
        InitializeComponent();
        DataContext = VM;
    }
}