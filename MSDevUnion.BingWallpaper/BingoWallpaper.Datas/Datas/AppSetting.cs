using BingoWallpaper.Models;
using BingoWallpaper.Services;
using SoftwareKobo.UniversalToolkit.Storage;
using System;
using System.Text.RegularExpressions;

namespace BingoWallpaper.Datas
{
    public static class AppSetting
    {
        /// <summary>
        /// 查看哪个国家/区域的壁纸。
        /// </summary>
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
                    return ServiceArea.DefaultArea;
                }
            }
            set
            {
                ApplicationRoamingSettings.Write(nameof(Area), value);
            }
        }

        /// <summary>
        /// 保存壁纸时，保存到哪个位置。
        /// </summary>
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

        /// <summary>
        /// 用户设置的壁纸大小。
        /// </summary>
        public static WallpaperSize WallpaperSize
        {
            get
            {
                if (ApplicationLocalSettings.Exists(nameof(WallpaperSize)))
                {
                    string value = ApplicationLocalSettings.Read<string>(nameof(WallpaperSize));
                    Regex regex = new Regex(@"^(\d+)x(\d+)$");
                    Match match = regex.Match(value);
                    if (match.Success)
                    {
                        GroupCollection groups = match.Groups;
                        int width = int.Parse(groups[1].Value);
                        int height = int.Parse(groups[2].Value);
                        return new WallpaperSize(width, height);
                    }
                }
                return null;
            }
            set
            {
                ApplicationLocalSettings.Write(nameof(WallpaperSize), value.ToString());
            }
        }

        private static readonly DateTimeOffset MIN_VIEW_MONTH = new DateTimeOffset(new DateTime(2015, 1, 1));

        /// <summary>
        /// 用户正在查看的哪个年月的壁纸。
        /// </summary>
        public static DateTimeOffset ViewMonth
        {
            get
            {
                DateTimeOffset value;
                if (ApplicationRoamingSettings.Exists(nameof(ViewMonth)))
                {
                    value = ApplicationRoamingSettings.Read<DateTimeOffset>(nameof(ViewMonth));
                }
                else
                {
                    value = DateTimeOffset.Now;
                }
                if (value < MIN_VIEW_MONTH)
                {
                    value = MIN_VIEW_MONTH;
                }
                else if (value > DateTimeOffset.Now)
                {
                    value = DateTimeOffset.Now;
                }
                return value;
            }
            set
            {
                ApplicationRoamingSettings.Write(nameof(ViewMonth), value);
            }
        }
    }
}