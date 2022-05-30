using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Hydra.Views.Controls;

/// <summary>
/// Interaction logic for ZoneListingView.xaml
/// </summary>
public partial class ZoneListingView
{
    public static readonly DependencyProperty IncomingTodoItemProperty =
        DependencyProperty.Register("IncomingTodoItem", typeof(object), typeof(ZoneListingView),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public object IncomingTodoItem
    {
        get => GetValue(IncomingTodoItemProperty);
        set => SetValue(IncomingTodoItemProperty, value);
    }

    public static readonly DependencyProperty RemovedTodoItemProperty =
        DependencyProperty.Register("RemovedTodoItem", typeof(object), typeof(ZoneListingView),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public object RemovedTodoItem
    {
        get => GetValue(RemovedTodoItemProperty);
        set => SetValue(RemovedTodoItemProperty, value);
    }

    public static readonly DependencyProperty TodoItemDropCommandProperty =
        DependencyProperty.Register("TodoItemDropCommand", typeof(ICommand), typeof(ZoneListingView),
            new PropertyMetadata(null));

    public ICommand TodoItemDropCommand
    {
        get => (ICommand)GetValue(TodoItemDropCommandProperty);
        set => SetValue(TodoItemDropCommandProperty, value);
    }

    public static readonly DependencyProperty TodoItemRemovedCommandProperty =
        DependencyProperty.Register("TodoItemRemovedCommand", typeof(ICommand), typeof(ZoneListingView),
            new PropertyMetadata(null));

    public ICommand TodoItemRemovedCommand
    {
        get => (ICommand)GetValue(TodoItemRemovedCommandProperty);
        set => SetValue(TodoItemRemovedCommandProperty, value);
    }

    public static readonly DependencyProperty TodoItemInsertedCommandProperty =
        DependencyProperty.Register("TodoItemInsertedCommand", typeof(ICommand), typeof(ZoneListingView),
            new PropertyMetadata(null));

    public ICommand TodoItemInsertedCommand
    {
        get => (ICommand)GetValue(TodoItemInsertedCommandProperty);
        set => SetValue(TodoItemInsertedCommandProperty, value);
    }

    public static readonly DependencyProperty InsertedTodoItemProperty =
        DependencyProperty.Register("InsertedTodoItem", typeof(object), typeof(ZoneListingView),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public object InsertedTodoItem
    {
        get => GetValue(InsertedTodoItemProperty);
        set => SetValue(InsertedTodoItemProperty, value);
    }

    public static readonly DependencyProperty TargetTodoItemProperty =
        DependencyProperty.Register("TargetTodoItem", typeof(object), typeof(ZoneListingView),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public object TargetTodoItem
    {
        get => GetValue(TargetTodoItemProperty);
        set => SetValue(TargetTodoItemProperty, value);
    }

    public ZoneListingView()
    {
        InitializeComponent();
    }

    private void ZoneItem_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed &&
            sender is FrameworkElement frameworkElement)
        {
            var todoItem = frameworkElement.DataContext;

            var dragDropResult = DragDrop.DoDragDrop(frameworkElement,
                new DataObject(DataFormats.Serializable, todoItem),
                DragDropEffects.Move);

            if (dragDropResult == DragDropEffects.None)
            {
                AddTodoItem(todoItem);
            }
        }
    }

    private void ZoneItem_DragOver(object sender, DragEventArgs e)
    {
        if (!TodoItemInsertedCommand.CanExecute(null)) return;
        if (sender is not FrameworkElement element) return;

        TargetTodoItem = element.DataContext;
        InsertedTodoItem = e.Data.GetData(DataFormats.Serializable) ?? new object();

        TodoItemInsertedCommand.Execute(null);
    }

    private void ZoneItemList_DragOver(object sender, DragEventArgs e)
    {
        var todoItem = e.Data.GetData(DataFormats.Serializable) ?? new object();
        AddTodoItem(todoItem);
    }

    private void AddTodoItem(object todoItem)
    {
        if (!TodoItemDropCommand.CanExecute(null)) return;

        IncomingTodoItem = todoItem;
        TodoItemDropCommand.Execute(null);
    }

    private void ZoneItemList_DragLeave(object sender, DragEventArgs e)
    {
        var result = VisualTreeHelper.HitTest(zoneListView, e.GetPosition(zoneListView));

        if (result != null) return;
        if (!TodoItemRemovedCommand.CanExecute(null)) return;

        RemovedTodoItem = e.Data.GetData(DataFormats.Serializable) ?? new object();
        TodoItemRemovedCommand.Execute(null);
    }
}