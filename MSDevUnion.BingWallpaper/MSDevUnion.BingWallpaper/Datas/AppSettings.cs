using Microsoft.Practices.ServiceLocation;
using MSDevUnion.BingWallpaper.Services;
using SoftwareKobo.UniversalToolkit.Storage;
using System;
using System.Globalization;
using System.Linq;

namespace MSDevUnion.BingWallpaper.Datas
{
    public static class AppSettings
    {
        public static string Area
        {
            get
            {
                if (ApplicationRoamingSettings.Exists(nameof(Area)))
                {
                    return ApplicationRoamingSettings.Read<string>(nameof(Area));
                }
                else
                {
                    string currentCulture = CultureInfo.CurrentCulture.Name;
                    IServiceArea serviceArea = ServiceLocator.Current.GetInstance<IServiceArea>();
                    if (serviceArea.AllSupportAreas.Contains(currentCulture))
                    {
                        return currentCulture;
                    }
                    else
                    {
                        return serviceArea.DefaultArea;
                    }
                }
            }
            set
            {
                ApplicationRoamingSettings.Write(nameof(Area), value);
            }
        }

        public static string SaveLocation
        {
            get
            {
                if (ApplicationRoamingSettings.Exists(nameof(SaveLocation)))
                {
                    return ApplicationRoamingSettings.Read<string>(nameof(SaveLocation));
                }
                else
                {
                    return LocalizedStrings.PictureLibrary;
                }
            }
            set
            {
                ApplicationRoamingSettings.Write(nameof(SaveLocation), value);
            }
        }

        public static WallpaperSize WallpaperSize
        {
            get
            {
                return ApplicationLocalSettings.Read<WallpaperSize>(nameof(WallpaperSize));
            }
            set
            {
                ApplicationLocalSettings.Write(nameof(WallpaperSize), value);
            }
        }

        /// <summary>
        /// 最后一次操作查看的年月。
        /// </summary>
        public static DateTimeOffset LastViewDate
        {
            get
            {
                DateTimeOffset returnValue;
                if (ApplicationRoamingSettings.Exists(nameof(LastViewDate)))
                {
                    returnValue = ApplicationRoamingSettings.Read<DateTimeOffset>(nameof(LastViewDate));
                }
                else
                {
                    var now = DateTimeOffset.Now;
                }
                if (returnValue <= new DateTime(2015, 1, 28))
                {
                    returnValue = new DateTime(2015, 1, 28);
                }
                else if(returnValue >= DateTimeOffset.Now)
                {
                    returnValue = DateTimeOffset.Now;
                }
                return returnValue;
            }
            set
            {
                ApplicationRoamingSettings.Write(nameof(LastViewDate), value);
            }
        }
    }
}