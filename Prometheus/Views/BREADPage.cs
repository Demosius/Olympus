using Prometheus.ViewModels.Helpers;
using System.Windows.Controls;

namespace Prometheus.Views;

/// <summary>
/// Base page class for all CRUD/BREAD base level data adjustment.
/// </summary>
public abstract class BREADPage : Page
{
    // Each one should reference a specific data type.
    public abstract EDataType DataType { get; }
}