using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Hydra.Views.Controls;

/// <summary>
/// Interaction logic for ZoneListingView.xaml
/// </summary>
public partial class ZoneListingView
{
    public static readonly DependencyProperty IncomingZoneItemProperty =
        DependencyProperty.Register("IncomingZoneItem", typeof(object), typeof(ZoneListingView),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public object IncomingZoneItem
    {
        get => GetValue(IncomingZoneItemProperty);
        set => SetValue(IncomingZoneItemProperty, value);
    }

    public static readonly DependencyProperty RemovedZoneItemProperty =
        DependencyProperty.Register("RemovedZoneItem", typeof(object), typeof(ZoneListingView),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public object RemovedZoneItem
    {
        get => GetValue(RemovedZoneItemProperty);
        set => SetValue(RemovedZoneItemProperty, value);
    }

    public static readonly DependencyProperty ZoneItemDropCommandProperty =
        DependencyProperty.Register("ZoneItemDropCommand", typeof(ICommand), typeof(ZoneListingView),
            new PropertyMetadata(null));

    public ICommand ZoneItemDropCommand
    {
        get => (ICommand)GetValue(ZoneItemDropCommandProperty);
        set => SetValue(ZoneItemDropCommandProperty, value);
    }

    public static readonly DependencyProperty ZoneItemRemovedCommandProperty =
        DependencyProperty.Register("ZoneItemRemovedCommand", typeof(ICommand), typeof(ZoneListingView),
            new PropertyMetadata(null));

    public ICommand ZoneItemRemovedCommand
    {
        get => (ICommand)GetValue(ZoneItemRemovedCommandProperty);
        set => SetValue(ZoneItemRemovedCommandProperty, value);
    }

    public static readonly DependencyProperty ZoneItemInsertedCommandProperty =
        DependencyProperty.Register("ZoneItemInsertedCommand", typeof(ICommand), typeof(ZoneListingView),
            new PropertyMetadata(null));

    public ICommand ZoneItemInsertedCommand
    {
        get => (ICommand)GetValue(ZoneItemInsertedCommandProperty);
        set => SetValue(ZoneItemInsertedCommandProperty, value);
    }

    public static readonly DependencyProperty InsertedZoneItemProperty =
        DependencyProperty.Register("InsertedZoneItem", typeof(object), typeof(ZoneListingView),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public object InsertedZoneItem
    {
        get => GetValue(InsertedZoneItemProperty);
        set => SetValue(InsertedZoneItemProperty, value);
    }

    public static readonly DependencyProperty TargetZoneItemProperty =
        DependencyProperty.Register("TargetZoneItem", typeof(object), typeof(ZoneListingView),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public object TargetZoneItem
    {
        get => GetValue(TargetZoneItemProperty);
        set => SetValue(TargetZoneItemProperty, value);
    }

    public ZoneListingView()
    {
        InitializeComponent();
    }

    private void ZoneItem_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed || sender is not FrameworkElement frameworkElement) return;

        var zoneItem = frameworkElement.DataContext;

        var dragDropResult = DragDrop.DoDragDrop(frameworkElement,
            new DataObject(DataFormats.Serializable, zoneItem),
            DragDropEffects.Move);

        if (dragDropResult == DragDropEffects.None)
        {
            AddZoneItem(zoneItem);
        }
    }

    private void ZoneItem_DragOver(object sender, DragEventArgs e)
    {
        if (!ZoneItemInsertedCommand.CanExecute(null)) return;
        if (sender is not FrameworkElement element) return;

        TargetZoneItem = element.DataContext;
        InsertedZoneItem = e.Data.GetData(DataFormats.Serializable) ?? new object();

        ZoneItemInsertedCommand.Execute(null);
    }

    private void ZoneItemList_DragOver(object sender, DragEventArgs e)
    {
        var zoneItem = e.Data.GetData(DataFormats.Serializable) ?? new object();
        AddZoneItem(zoneItem);
    }

    private void AddZoneItem(object zoneItem)
    {
        if (!ZoneItemDropCommand.CanExecute(null)) return;

        IncomingZoneItem = zoneItem;
        ZoneItemDropCommand.Execute(null);
    }

    private void ZoneItemList_DragLeave(object sender, DragEventArgs e)
    {
        var result = VisualTreeHelper.HitTest(ZoneListView, e.GetPosition(ZoneListView));

        if (result != null) return;
        if (!ZoneItemRemovedCommand.CanExecute(null)) return;

        RemovedZoneItem = e.Data.GetData(DataFormats.Serializable) ?? new object();
        ZoneItemRemovedCommand.Execute(null);
    }
}