using Aion.ViewModel;

namespace Aion.View
{
    /// <summary>
    /// Interaction logic for DateRangeWindow.xaml
    /// </summary>
    public partial class DateRangeWindow
    {
        public DateRangeWindow(ShiftEntryPageVM editorVM)
        {
            InitializeComponent();
            VM.ShiftEntryPageVM = editorVM;
        }
    }
}
