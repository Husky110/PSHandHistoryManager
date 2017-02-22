using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSHandManagerLib
{
    public class HandProcessor
    {
        [ThreadStatic] public static bool shutdown = false;
        [ThreadStatic] public static int taskcounter = 0;
        public HandProcessor(string pathToAppConfig)
        {
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", pathToAppConfig);
        }
    }
}
