using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace BingoWallpaper.ThirdParty
{
    public class VirtualizingFlipView : FlipView
    {
        public VirtualizingFlipView()
        {
            this.DefaultStyleKey = typeof(VirtualizingFlipView);

            this.SelectionChanged += VirtualizingFlipView_SelectionChanged;

            ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
        }

        private void ItemContainerGenerator_ItemsChanged(object sender, Windows.UI.Xaml.Controls.Primitives.ItemsChangedEventArgs e)
        {
            foreach (var item in Items)
            {
                var container = ContainerFromItem(item);
                if (container == null)
                {
                    continue;
                }

                (container as VirtualizingFlipViewItem).IsVirtualized = item != SelectedItem;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new VirtualizingFlipViewItem();
        }
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is VirtualizingFlipViewItem;
        }

        private async void VirtualizingFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var oldContainer = e.RemovedItems.Count > 0 ? ContainerFromItem(e.RemovedItems.FirstOrDefault()) as VirtualizingFlipViewItem: null;
            
            if (this.SelectedItem != null)
            {
                var newContainer = ContainerFromIndex(this.SelectedIndex) as VirtualizingFlipViewItem;
                if (newContainer == null)
                {
                    await Task.Delay(500);
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        newContainer = ContainerFromIndex(this.SelectedIndex) as VirtualizingFlipViewItem;
                        newContainer?.Realize();
                    });
                }
                else
                {
                    RealizeItem(newContainer);
                }
            }
            
            if (oldContainer != null)
            {
                VirtualizeItem(oldContainer);
            }
        }

        private VirtualizingFlipViewItem _nextVirtualizingItem;
        private Task _virtualizeItemTask;
        private CancellationTokenSource _virtualizeItemTaskCancellationTokenSource;
        private void VirtualizeItem(VirtualizingFlipViewItem item)
        {
            if (_nextVirtualizingItem != null)
            {
                _virtualizeItemTaskCancellationTokenSource?.Cancel();
                _nextVirtualizingItem.Virtualize();
                _virtualizeItemTask = null;
                _virtualizeItemTaskCancellationTokenSource = null;
            }
            _nextVirtualizingItem = item;
            _virtualizeItemTaskCancellationTokenSource = new CancellationTokenSource();
            _virtualizeItemTask = new Task(ToVirtualizeItem, _virtualizeItemTaskCancellationTokenSource.Token);
            _virtualizeItemTask.Start();
        }

        private async void ToVirtualizeItem()
        {
            var item = _nextVirtualizingItem;
            await Task.Delay(1000);
            if (item != null && item == _nextVirtualizingItem)
            {
                await item.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    item.Virtualize();
                    item = null;
                });
            }
        }

        private async void RealizeItem(VirtualizingFlipViewItem item)
        {
            if (_nextVirtualizingItem == item)
            {
                _virtualizeItemTaskCancellationTokenSource?.Cancel();
                _nextVirtualizingItem = null;
                _virtualizeItemTask = null;
                _virtualizeItemTaskCancellationTokenSource = null;
            }
            await item.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, item.Realize);
        }
    }
}
