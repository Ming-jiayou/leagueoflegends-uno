using System.Collections;
using Microsoft.UI.Xaml.Input;

namespace Jamesnet.Uno;

public class RecursiveControlItem : Control
{
    public RecursiveControlItem()
    {
        DefaultStyleKey = typeof(RecursiveControlItem);
    }

    public object ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(RecursiveControlItem), new PropertyMetadata(null, OnItemsSourceChanged));

    public string ItemsBindingPath
    {
        get => (string)GetValue(ItemsBindingPathProperty);
        set => SetValue(ItemsBindingPathProperty, value);
    }

    public static readonly DependencyProperty ItemsBindingPathProperty =
        DependencyProperty.Register(nameof(ItemsBindingPath), typeof(string), typeof(RecursiveControlItem), new PropertyMetadata(null));

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((RecursiveControlItem)d).GenerateItems();
    }

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public static readonly DependencyProperty IsExpandedProperty =
        DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(RecursiveControlItem), new PropertyMetadata(true, OnIsExpandedChanged));

    private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((RecursiveControlItem)d).UpdateChildrenVisibility();
    }

    private Panel _itemsPanel;

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _itemsPanel = GetTemplateChild("PART_ItemsPanel") as Panel;
        UpdateChildrenVisibility();
        SetItemsSourceFromDataContext();

        if (GetTemplateChild("PART_Root") is FrameworkElement root)
        {
            root.DoubleTapped += OnDoubleTapped;
        }
    }

    private void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        IsExpanded = !IsExpanded;
        e.Handled = true;
    }

    private void SetItemsSourceFromDataContext()
    {
        if (DataContext == null || string.IsNullOrEmpty(ItemsBindingPath)) return;

        var dataContextType = DataContext.GetType();
        var property = dataContextType.GetProperty(ItemsBindingPath);

        if (property != null && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
        {
            var value = property.GetValue(DataContext);
            if (value is IEnumerable enumerable)
            {
                ItemsSource = enumerable;
            }
        }
    }

    private void GenerateItems()
    {
        if (_itemsPanel == null || ItemsSource == null) return;

        _itemsPanel.Children.Clear();

        foreach (var item in ItemsSource as IEnumerable)
        {
            var container = GetContainerForItem();
            container.DataContext = item;
            container.ItemsBindingPath = ItemsBindingPath;
            _itemsPanel.Children.Add(container);
        }
    }

    protected virtual RecursiveControlItem GetContainerForItem()
    {
        return new RecursiveControlItem();
    }

    private void UpdateChildrenVisibility()
    {
        if (_itemsPanel != null)
        {
            _itemsPanel.Visibility = IsExpanded ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}