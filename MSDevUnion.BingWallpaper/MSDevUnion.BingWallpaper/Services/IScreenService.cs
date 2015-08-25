using System.Threading.Tasks;

namespace MSDevUnion.BingWallpaper.Services
{
    public interface IScreenService
    {
        int Width
        {
            get;
        }

        int Height
        {
            get;
        }

        Task InitAsync();
    }
}