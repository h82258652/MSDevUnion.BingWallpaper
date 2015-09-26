using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BingoWallpaper.ThirdParty
{
    interface IVirtualizingItem
    {
        bool IsVirtualized { get; }

        bool CanVirtualize { get; }

        bool CanRealize { get; }

        void Virtualize();

        void Realize();
    }
}
