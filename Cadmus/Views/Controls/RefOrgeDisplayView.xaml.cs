﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Cadmus.ViewModels.Controls;
using Cadmus.ViewModels.Labels;
using Uranus;

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
