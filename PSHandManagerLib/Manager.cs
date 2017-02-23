using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSHandManagerLib.FileSystem;
using System.Threading;
using System.Diagnostics;
using PSHandManagerLib.Exceptions;

namespace PSHandManagerLib
{
    public class Manager
    {
        [ThreadStatic] public static bool shutdown = false;
        [ThreadStatic] public static string culture = "";
        [ThreadStatic] public static string appPath = "";

        public static ConcurrentDictionary<Task, bool> runningTasks = new ConcurrentDictionary<Task, bool>(); //used to monitor all tasks in case of shutdown and for GUI-data
        public Manager(string pathToAppConfig, string appPath)
        {
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", pathToAppConfig);
            Manager.culture = ConfigurationManager.AppSettings["Language"]; //TODO: Bug -> right after setup this value is not set. restart of program loads is correct
            Manager.appPath = appPath;
            ManagerException.initializeLocalizedErrorMessages();
        }

        public void run()
        {
            FileSystemWatcher fsw = new FileSystemWatcher();
            Task fswTask = new Task(() => fsw.run());
            fswTask.Start();
            Manager.runningTasks.TryAdd(fswTask, true);
            while (shutdown)
            {
                Thread.Sleep(1000); //this one has to do nothing...
            }
            // shutdown - wait for all tasks to finish...
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while(Manager.runningTasks.Keys.Count > 0 && sw.Elapsed.Seconds < 3)
            {
                Thread.Sleep(100); //check every 100ms if all tasks have finished after 3 seconds -> hard shutdown
            }
        }
    }
}
