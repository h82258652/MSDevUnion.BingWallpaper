using System.Collections.Generic;

namespace MSDevUnion.BingWallpaper.Services
{
    public interface IServiceArea
    {
        IReadOnlyList<string> AllSupportAreas
        {
            get;
        }

        string DefaultArea
        {
            get;
        }
    }
}