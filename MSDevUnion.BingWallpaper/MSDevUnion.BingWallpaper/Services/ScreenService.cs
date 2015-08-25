using System;
using System.Threading.Tasks;

namespace MSDevUnion.BingWallpaper.Services
{
    public class ScreenService : IScreenService
    {
        private static int _width;

        private static int _height;

        private static bool _hadInit;

        public int Height
        {
            get
            {
                if (_hadInit == false)
                {
                    throw new InvalidOperationException("Not Init");
                }
                return _height;
            }
        }

        public int Width
        {
            get
            {
                if (_hadInit == false)
                {
                    throw new InvalidOperationException("Not Init");
                }
                return _width;
            }
        }

        public async Task InitAsync()
        {
            _width = await SoftwareKobo.UniversalToolkit.Helpers.ScreenResolutionHelper.GetWidthAsync();
            _height = await SoftwareKobo.UniversalToolkit.Helpers.ScreenResolutionHelper.GetHeightAsync();
            _hadInit = true;
        }
    }
}