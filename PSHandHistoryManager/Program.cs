using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Configuration;
using System.Threading;

namespace PSHandHistoryManager
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string configFilePath = Application.StartupPath + "\\App.config";
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", configFilePath);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (ConfigurationManager.AppSettings["Language"] != "")
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(ConfigurationManager.AppSettings["Language"]);
            }
            Application.Run(new MainForm(configFilePath));
        }
    }
}
