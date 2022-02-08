using Uranus;

namespace Aion.View
{
    public partial class ClockCreationWindow
    {
        public ClockCreationWindow(Helios helios)
        {
            InitializeComponent();
            VM.SetDataSource(helios);
        }
    }
}
