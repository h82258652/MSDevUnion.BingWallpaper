using Windows.Foundation.Metadata;

namespace MSDevUnion.BingWallpaper.Utils
{
    public static class HardwareButtonsHelper
    {
        public static bool IsPresent
        {
            get
            {
                return ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons");
            }
        }
    }
}