using Prometheus.ViewModels.Helpers;
using System.Windows.Controls;

namespace Prometheus.Views;

/// <summary>
/// Category page for grouping this general type of page, to be treated separately than a standard page.
/// </summary>
public abstract class CatPage : Page
{
    // Each one should reference a specific data category.
    public abstract EDataCategory DataCategory { get; }
}