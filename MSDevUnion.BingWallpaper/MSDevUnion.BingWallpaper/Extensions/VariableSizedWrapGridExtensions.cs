using System.Collections;
using System.Collections.Specialized;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MSDevUnion.BingWallpaper.Extensions
{
    public static class VariableSizedWrapGridExtensions
    {
        public static void SetItemsSource(VariableSizedWrapGrid obj, object value)
        {
            obj.SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.RegisterAttached("ItemsSource", typeof(object), typeof(VariableSizedWrapGrid), new PropertyMetadata(null, ItemsSourceChanged));

        public static object GetItemsSource(VariableSizedWrapGrid obj)
        {
            return obj.GetValue(ItemsSourceProperty);
        }

        public static DataTemplate GetItemTemplate(VariableSizedWrapGrid obj)
        {
            return (DataTemplate)obj.GetValue(ItemTemplateProperty);
        }

        public static void SetItemTemplate(VariableSizedWrapGrid obj, DataTemplate value)
        {
            obj.SetValue(ItemTemplateProperty, value);
        }

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.RegisterAttached("ItemTemplate", typeof(DataTemplate), typeof(VariableSizedWrapGridExtensions), new PropertyMetadata(null, ItemTemplateChanged));

        private static void ResetGrid(VariableSizedWrapGrid grid)
        {
            grid.Children.Clear();
            var itemsSource = GetItemsSource(grid) as IEnumerable;
            if (itemsSource != null)
            {
                DataTemplate template = GetItemTemplate(grid);
                foreach (var item in itemsSource)
                {
                    UIElement child = (UIElement)template.LoadContent();
                    FrameworkElement element = child as FrameworkElement;
                    if (element != null)
                    {
                        element.DataContext = item;
                    }
                    grid.Children.Add(child);
                }
            }
        }

        private static void ItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ResetGrid((VariableSizedWrapGrid)d);
        }

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ResetGrid((VariableSizedWrapGrid)d);
            if (e.NewValue is INotifyCollectionChanged)
            {
                var collection = e.NewValue as INotifyCollectionChanged;
                collection.CollectionChanged += delegate
                {
                    ResetGrid((VariableSizedWrapGrid)d);
                };
            }
        }
    }
}