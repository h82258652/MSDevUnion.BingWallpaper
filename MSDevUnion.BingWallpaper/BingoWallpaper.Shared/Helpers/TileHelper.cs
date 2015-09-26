using BingoWallpaper.Models;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace BingoWallpaper.Helpers
{
    internal class TileHelper
    {
        public static XmlDocument CreateTileTemplate(Wallpaper wallpaper)
        {
            XmlDocument document = new XmlDocument();

            // tile 根节点。
            XmlElement tile = document.CreateElement("tile");
            document.AppendChild(tile);

            // visual 元素。
            XmlElement visual = document.CreateElement("visual");
            tile.AppendChild(visual);

            // Medium,150x150。
            {
                // binding
                XmlElement binding = document.CreateElement("binding");
                binding.SetAttribute("template", "TileMedium");
                visual.AppendChild(binding);

                // image
                XmlElement image = document.CreateElement("image");
                image.SetAttribute("src", wallpaper.GetOriginalUrl(new WallpaperSize(150, 150)));
                image.SetAttribute("placement", "background");
                binding.AppendChild(image);

                // text
                XmlElement text = document.CreateElement("text");
                text.AppendChild(document.CreateTextNode(wallpaper.Archive.Info));
                text.SetAttribute("hint-wrap", "true");
                binding.AppendChild(text);
            }

            // Wide,310x150。
            {
                // binding
                XmlElement binding = document.CreateElement("binding");
                binding.SetAttribute("template", "TileWide");
                visual.AppendChild(binding);

                // image
                XmlElement image = document.CreateElement("image");
                image.SetAttribute("src", wallpaper.GetOriginalUrl(new WallpaperSize(310, 150)));
                image.SetAttribute("placement", "background");
                binding.AppendChild(image);

                // text
                XmlElement text = document.CreateElement("text");
                text.AppendChild(document.CreateTextNode(wallpaper.Archive.Info));
                text.SetAttribute("hint-wrap", "true");
                binding.AppendChild(text);
            }

            return document;
        }

        public static void UpdatePrimaryTile(Wallpaper wallpaper)
        {
            TileNotification tile = new TileNotification(CreateTileTemplate(wallpaper));
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tile);
        }
    }
}