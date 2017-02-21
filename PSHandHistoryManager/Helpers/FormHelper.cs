using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace PSHandHistoryManager.Helpers
{
    class FormHelper
    {

        public static string getPokerStarsPath()
        {
            // Code from http://stackoverflow.com/questions/24909108/get-installed-software-list-using-c-sharp
            // modified by Husky110
            string[] keys = new string[2] { @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"};
            for (int x = 0; x < 2; x++)
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(keys[x]);
                foreach (String keyName in key.GetSubKeyNames())
                {
                    RegistryKey subkey = key.OpenSubKey(keyName);
                    string displayName = subkey.GetValue("DisplayName") as string;
                    if(displayName != null && displayName.Contains("PokerStars"))
                    {
                        return subkey.GetValue("InstallLocation") + "\\PokerStars.exe";
                    }
                }
            }
            return "1";
        }
        public static Dictionary<String, String> GetSupportedCultures()
        {
            //original code from: http://stackoverflow.com/questions/553244/programmatic-way-to-get-all-the-available-languages-in-satellite-assemblies
            //modified by Husky110

            //Get all culture 
            CultureInfo[] culture = CultureInfo.GetCultures(CultureTypes.AllCultures);

            //Find the location where application installed.
            string exeLocation = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));

            //Return all culture for which satellite folder found with culture code.
            IEnumerable<CultureInfo> supported = culture.Where(cultureInfo => Directory.Exists(Path.Combine(exeLocation, cultureInfo.Name)));

            Dictionary<String, String> retval = new Dictionary<string, string>();
            foreach(CultureInfo ci in supported)
            {
                if(ci.LCID == 127) { continue; } // we don't need the invariant here...
                retval.Add(ci.Parent.NativeName, ci.Name);
            }

            return retval;
        }
    }
}
