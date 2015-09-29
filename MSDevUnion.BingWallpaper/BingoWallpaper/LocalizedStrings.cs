using Windows.ApplicationModel.Resources;

namespace BingoWallpaper
{
    public static partial class LocalizedStrings
    {
        public static string Australia
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Area").GetString("Australia");
            }
        }

        public static string Brazil
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Area").GetString("Brazil");
            }
        }

        public static string Canada_English
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Area").GetString("Canada_English");
            }
        }

        public static string Canada_French
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Area").GetString("Canada_French");
            }
        }

        public static string China
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Area").GetString("China");
            }
        }

        public static string France
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Area").GetString("France");
            }
        }

        public static string Germany
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Area").GetString("Germany");
            }
        }

        public static string India
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Area").GetString("India");
            }
        }

        public static string Japan
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Area").GetString("Japan");
            }
        }

        public static string United_Kingdom
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Area").GetString("United_Kingdom");
            }
        }

        public static string United_States
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Area").GetString("United_States");
            }
        }

        public static string SaveFailed
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Detail").GetString("SaveFailed");
            }
        }

        public static string SaveSuccess
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Detail").GetString("SaveSuccess");
            }
        }

        public static string SetFailed
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Detail").GetString("SetFailed");
            }
        }

        public static string SetSuccess
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Detail").GetString("SetSuccess");
            }
        }

        public static string ShareFailed
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Detail").GetString("ShareFailed");
            }
        }

        public static string ShareSuccess
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Detail").GetString("ShareSuccess");
            }
        }

        public static string Description
        {
            get
            {
                return ResourceLoader.GetForCurrentView().GetString("Description");
            }
        }

        public static string DisplayName
        {
            get
            {
                return ResourceLoader.GetForCurrentView().GetString("DisplayName");
            }
        }

        public static string ChooseEveryTime
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Setting").GetString("ChooseEveryTime");
            }
        }

        public static string PictureLibrary
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Setting").GetString("PictureLibrary");
            }
        }

        public static string SavedPictures
        {
            get
            {
                return ResourceLoader.GetForCurrentView("Setting").GetString("SavedPictures");
            }
        }
    }
}
