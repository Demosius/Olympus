using Aion.ViewModel;
using Uranus;

namespace Aion.View
{
    /// <summary>
    /// Interaction logic for SimpleEntryCreationView.xaml
    /// </summary>
    public partial class SimpleEntryCreationWindow
    {
        public SimpleEntryCreationWindow(Helios helios, ShiftEntryPageVM editorVM)
        {
            InitializeComponent();
            VM.SetData(helios, editorVM);
        }
    }
}
