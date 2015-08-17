using System.Threading.Tasks;

namespace MSDevUnion.BingWallpaper.Services
{
    public interface IScreenService
    {
        Task<int> GetScreenWidthAsync();

        Task<int> GetScreenHeightAsync();
    }
}