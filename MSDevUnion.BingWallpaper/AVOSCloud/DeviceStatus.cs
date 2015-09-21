using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVOSCloud
{
    public class DeviceStatus 
    {
        public static string DeviceName
        {
            get
            {
                var i = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
                return i.SystemProductName;
            }
        }

        public static string DeviceManufaturer
        {
            get
            {
                var i = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
                return i.SystemManufacturer;
            }
        }

        public static long DeviceTotalMemory
        {
            get
            {
                // TODO: Can I get this info?
                return -1;
            }
        }
    }
}
