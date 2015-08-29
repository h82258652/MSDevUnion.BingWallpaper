using MSDevUnion.BingWallpaper.Models;
using System.Threading.Tasks;

namespace MSDevUnion.BingWallpaper.Services
{
    public interface IBingWallpaperService
    {
        Task<LeanCloudResultCollection<Archive>> GetArchivesAsync(int year, int month, string market);

        Task<Image> GetImageAsync(string objectId);
    }
}