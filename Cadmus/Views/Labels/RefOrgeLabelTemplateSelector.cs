using System.Windows;
using System.Windows.Controls;
using Cadmus.ViewModels.Labels;

namespace Cadmus.Views.Labels;

public class RefOrgeLabelTemplateSelector : DataTemplateSelector
{
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (container is not FrameworkElement element || item is not RefOrgeLabelVM labelVM) return null;

        if (labelVM.IsMixed)
            return element.FindResource("MixedCartonTemplate") as DataTemplate;

        return element.FindResource("RefOrgeTemplate") as DataTemplate;
    }
}