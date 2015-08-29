using Microsoft.Practices.ServiceLocation;
using MSDevUnion.BingWallpaper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MSDevUnion.BingWallpaper.Services
{
    public static class PackagedBingWallpaperService
    {
        public static async Task<ObservableCollection<BingWallpaperModel>> GetAsync(string market, int year, int month)
        {
            try
            {
                var service = ServiceLocator.Current.GetInstance<IBingWallpaperService>();
                var archiveCollection = await service.GetArchivesAsync(year, month, market);
                if (archiveCollection.ErrorCode != 0)
                {
                    return new ObservableCollection<BingWallpaperModel>();
                }
                else
                {
                    var archive = archiveCollection.Results;

                    List<BingWallpaperModel> bingWallpapers = new List<BingWallpaperModel>();

                    for (int i = 0; i < archive.Length; i++)
                    {
                        BingWallpaperModel bingWallpaper = new BingWallpaperModel();

                        #region 包装 Archive 的属性

                        bingWallpaper.Hotspots = archive[i].Hotspots;
                        bingWallpaper.Info = archive[i].Info;

                        #endregion 包装 Archive 的属性

                        var image = await service.GetImageAsync(archive[i].Image.ObjectId);

                        if (image.ErrorCode == 0)
                        {
                            #region 包装 Image 的属性

                            bingWallpaper.UrlBase = image.UrlBase;
                            bingWallpaper.ExistWUXGA = image.ExistWUXGA;

                            #endregion 包装 Image 的属性
                        }

                        bingWallpapers.Add(bingWallpaper);
                    }

                    return new ObservableCollection<BingWallpaperModel>(bingWallpapers);
                }
            }
            catch (Exception ex)
            {
                return new ObservableCollection<BingWallpaperModel>();
            }
        }
    }
}