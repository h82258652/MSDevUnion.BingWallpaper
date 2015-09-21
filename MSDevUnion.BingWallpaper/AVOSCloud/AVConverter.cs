using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AVOSCloud
{
    public static class AVConverter
    {
        public static void DeserializeObject<T>(this AVObject avObject)
        {
            Type t = typeof (T);
            var o = Activator.CreateInstance<T>();
            //var ot=
            
            var ps = t.GetRuntimeProperties();
            foreach (var p in ps)
            {
                int c = 0;
            }
            var i = 0;
        }
    }
}
