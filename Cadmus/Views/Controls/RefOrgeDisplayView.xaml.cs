using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cadmus.ViewModels.Controls;
using Cadmus.ViewModels.Labels;
using Uranus;
using Point = System.Windows.Point;

// ReSharper disable StringLiteralTypo

namespace Cadmus.Views.Controls;

/// <summary>
/// Interaction logic for RefOrgeView.xaml
/// </summary>
public partial class RefOrgeDisplayView
{
    public RefOrgeDisplayView(Helios helios)
    {
        InitializeComponent();
        DataContext = new RefOrgeDisplayVM(helios);
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var vm = (RefOrgeDisplayVM)DataContext;
        vm.SelectedLabels =
            new ObservableCollection<RefOrgeLabelVM>(LabelViewList.SelectedItems
                .Cast<RefOrgeLabelVM>()
                .ToList());
    }

    private void MasterSelection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var vm = (RefOrgeDisplayVM)DataContext;
        vm.SelectedMasterLabels =
            new ObservableCollection<RefOrgeMasterLabelVM>(MasterLabelGrid.SelectedItems
                .Cast<RefOrgeMasterLabelVM>()
                .ToList());
    }
}
