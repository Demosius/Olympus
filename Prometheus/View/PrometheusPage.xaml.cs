using System;
using Styx;
using Uranus;
using Uranus.Staff;

namespace Prometheus.View
{
    /// <summary>
    /// Interaction logic for Prometheus.xaml
    /// </summary>
    public partial class PrometheusPage : IProject
    {
        public PrometheusPage(Helios helios, Charon charon)
        {
            InitializeComponent();
            VM.SetDataSources(helios, charon);
        }

        public EProject Project => EProject.Prometheus;

        public static bool RequiresUser => false;

        public void RefreshData()
        {
            throw new NotImplementedException();
        }

    }
}
