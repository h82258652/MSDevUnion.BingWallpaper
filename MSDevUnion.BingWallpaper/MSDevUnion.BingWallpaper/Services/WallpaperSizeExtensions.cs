namespace MSDevUnion.BingWallpaper.Services
{
    public static class WallpaperSizeExtensions
    {
        public static string GetName(this WallpaperSize size)
        {
            return size.ToString().TrimStart('_');
        }

        public static int GetWidth(this WallpaperSize size)
        {
            return int.Parse(size.GetName().Split('x')[0]);
        }

        public static int GetHeight(this WallpaperSize size)
        {
            return int.Parse(size.GetName().Split('x')[1]);
        }
    }
}