using System.Windows;
using System.Windows.Controls;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.TemplateSelector;

internal class RosterTemplateSelector : DataTemplateSelector
{
    public DataTemplate EmployeeTemplate { get; set; }

    public DataTemplate RosterTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        return item switch
        {
            Roster => RosterTemplate,
            Employee => EmployeeTemplate,
            _ => base.SelectTemplate(item, container) ?? new DataTemplate()
        };
    }
}