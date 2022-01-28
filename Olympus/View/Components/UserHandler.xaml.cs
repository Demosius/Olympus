using Olympus.Properties;
using Styx;


namespace Olympus.View.Components
{
    /// <summary>
    /// Interaction logic for SignIn.xaml
    /// </summary>
    public partial class UserHandler
    {
        public Charon Charon { get; set; }

        public UserHandler()
        {
            InitializeComponent();
            Charon = new(Settings.Default.SolLocation);
        }
    }
}
