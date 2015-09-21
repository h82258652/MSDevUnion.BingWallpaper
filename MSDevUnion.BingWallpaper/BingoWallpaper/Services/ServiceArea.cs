using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BingoWallpaper.Services
{
    public static class ServiceArea
    {
        public static IReadOnlyList<string> All
        {
            get
            {
                string[] list = new string[]
                {
                    "de-DE",
                    "en-AU",
                    "en-CA",
                    "en-GB",
                    "en-IN",
                    "en-US",
                    "fr-CA",
                    "fr-FR",
                    "ja-JP",
                    "pt-BR",
                    "zh-CN"
                };
                return list;
            }
        }

        public static string DefaultArea
        {
            get
            {
                string currentArea = CultureInfo.CurrentCulture.Name;
                if (All.Contains(currentArea, StringComparer.OrdinalIgnoreCase))
                {
                    return currentArea;
                }
                return "en-US";
            }
        }
    }
}