using Prometheus.ViewModel.Helpers;
using System.Windows.Controls;

namespace Prometheus.View
{
    /// <summary>
    /// Base page class for all CRUD/BREAD based pages for adjusting data.
    /// </summary>
    public abstract class BREADBase : Page
    {
        // Each one should reference a specific data type.
        public abstract EDataType DataType { get; }
    }
}
