using System;

namespace BingoWallpaper.Models
{
    public class WallpaperSize : IEquatable<WallpaperSize>
    {
        private int _height;

        private int _width;

        public WallpaperSize(int width, int height)
        {
            this._width = width;
            this._height = height;
        }

        public static WallpaperSize[] SupportWallpaperSizes
        {
            get
            {
                var array = new WallpaperSize[]
                {
                    //new WallpaperSize(150,150),
                    //new WallpaperSize(200,200),
                    //new WallpaperSize(240,240),
                    //new WallpaperSize(240,400),
                    //new WallpaperSize(310,150),
                    //new WallpaperSize(320,180),
                    //new WallpaperSize(400,240),
                    new WallpaperSize(480,800),
                    //new WallpaperSize(640,360),
                    //new WallpaperSize(720,1280),
                    //new WallpaperSize(768,1024),
                    new WallpaperSize(768,1280),
                    //new WallpaperSize(768,1366),
                    new WallpaperSize(800,480),
                    //new WallpaperSize(800,600),
                    //new WallpaperSize(1024,768),
                    new WallpaperSize(1080,1920),
                    //new WallpaperSize(1280,720),
                    //new WallpaperSize(1280,768),
                    new WallpaperSize(1366,768),
                    new WallpaperSize(1920,1080),
                    new WallpaperSize(1920,1200)
                };
                return array;
            }
        }

        public int Height
        {
            get
            {
                return this._height;
            }
        }

        public int Width
        {
            get
            {
                return this._width;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return this == null;
            }

            WallpaperSize other = obj as WallpaperSize;
            if (other == null)
            {
                return false;
            }
            else
            {
                return this.Width == other.Width && this.Height == other.Height;
            }
        }

        public bool Equals(WallpaperSize other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Width == other.Width && this.Height == other.Height;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}x{1}", Width, Height);
        }
    }
}