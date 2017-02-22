﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSHandManagerLib.FileSystem;
using System.Threading;
using System.Diagnostics;

namespace PSHandManagerLib
{
    public class HandProcessor
    {
        [ThreadStatic] public static bool shutdown = false;
        public static ConcurrentDictionary<Task, bool> runningTasks = new ConcurrentDictionary<Task, bool>(); //used to monitor all tasks in case of shutdown and for GUI-data
        public HandProcessor(string pathToAppConfig)
        {
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", pathToAppConfig);
        }

        public void run()
        {
            FileSystemWatcher fsw = new FileSystemWatcher();
            Task fswTask = new Task(() => fsw.run());
            fswTask.Start();
            HandProcessor.runningTasks.TryAdd(fswTask, true);
            while (shutdown)
            {
                Thread.Sleep(1000); //this one has to do nothing...
            }
            // shutdown - wait for all tasks to finish...
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while(HandProcessor.runningTasks.Keys.Count > 0 && sw.Elapsed.Seconds < 3)
            {
                Thread.Sleep(100); //check every 100ms if all tasks have finished after 3 seconds -> hard shutdown
            }
        }
    }
}
