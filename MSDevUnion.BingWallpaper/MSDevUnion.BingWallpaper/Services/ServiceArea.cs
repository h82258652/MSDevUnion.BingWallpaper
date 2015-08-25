using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MSDevUnion.BingWallpaper.Services
{
    public class ServiceArea : IServiceArea
    {
        private static readonly IReadOnlyList<string> _supportAreas = new string[]
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

        public IReadOnlyList<string> AllSupportAreas
        {
            get
            {
                return _supportAreas;
            }
        }

        public string DefaultArea
        {
            get
            {
                string currentArea = CultureInfo.CurrentCulture.Name;
                if (this.AllSupportAreas.Contains(currentArea, StringComparer.OrdinalIgnoreCase))
                {
                    // user in support country/zone.
                    return currentArea;
                }
                return "en-US";
            }
        }
    }
}