using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDevUnion.BingWallpaper.Datas
{
    public class ApplicationSetting
    {
        private Services.ILocalSetting _localSetting;
        private Services.IRoamingSetting _roamingSetting;

        public string Area
        {
            get
            {
                return _roamingSetting.Load(nameof(Area));
            }
            set
            {
                _roamingSetting.Save(nameof(Area), value);
            }
        }

        public string SaveLocation
        {
            get
            {
                return _roamingSetting.Load(nameof(SaveLocation));
            }
            set
            {
                _roamingSetting.Save(nameof(SaveLocation), value);
            }
        }
    }
}
