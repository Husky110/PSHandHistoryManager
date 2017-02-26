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
using PSHandManagerLib.Localizations;
using PSHandManagerLib.HandProcessing;

namespace PSHandManagerLib
{
    /// <summary>
    /// Startpoint of all operations
    /// This class initializes all working threads and holds neccesary values
    /// </summary>
    public class Manager
    {
        /// <summary>
        /// If set to true, the Manager will finalize all working threads and close itself.
        /// </summary>
        public static bool shutdown = false;
        /// <summary>
        /// Hold the current culture. Set here to avoid Deadlocks on Configuration
        /// </summary>
        public static string culture = "";
        /// <summary>
        /// Holds the path to the root of PSHandhistoryManager -> mainly used in ExceptionHandling to get the localized errormessages
        /// </summary>
        public static string appPath = "";

        /// <summary>
        /// Used to monitor all tasks in case of shutdown and for GUI-data
        /// </summary>
        public static ConcurrentDictionary<Task, bool> runningTasks = new ConcurrentDictionary<Task, bool>();
        /// <summary>
        /// Initalizes the Manager
        /// </summary>
        /// <param name="appPath">Value of Application.StartupPath</param>
        public Manager(string appPath)
        {
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", appPath + "\\App.config");
            Manager.culture = ConfigurationManager.AppSettings["Language"];
            Manager.appPath = appPath;
            ManagerException.initializeLocalizedErrorMessages();
            HandLanguageDetector.initializeHandLanguageDetector();
        }

        /// <summary>
        /// Starts and runs the magic
        /// </summary>
        public void run()
        {
            //always initiate the HandProcessDispatcher BEFORE the FileSystemWatcher...
            HandProcessDispatcher hpd = new HandProcessDispatcher();
            Task hpdTask = new Task(() => hpd.run());
            hpdTask.Start();
            Manager.runningTasks.TryAdd(hpdTask, true);
            FileSystemWatcher fsw = new FileSystemWatcher();
            Task fswTask = new Task(() => fsw.run()); // Start this task only once for now! TODO: start this task multiple times to scan folders of multiple users on the same machine
            fswTask.Start();
            Manager.runningTasks.TryAdd(fswTask, true);
            while (Manager.shutdown == false)
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
