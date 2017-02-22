using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSHandManagerLib.FileSystem;
using System.Threading;

namespace PSHandManagerLib
{
    public class HandProcessor
    {
        [ThreadStatic] public static bool shutdown = false;
        [ThreadStatic] public static int taskcounter = 0;
        public List<Task> runningTasks = new List<Task>();
        public HandProcessor(string pathToAppConfig)
        {
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", pathToAppConfig);
        }

        public void run()
        {
            FileSystemWatcher fsw = new FileSystemWatcher();
            Task fswTask = new Task(() => fsw.run());
            fswTask.Start();
            runningTasks.Add(fswTask);
            while (shutdown)
            {
                Thread.Sleep(1000); //this one has to do nothing...
            }
            // shutdown - wait for all tasks to finish...
            foreach (Task t in this.runningTasks)
            {
                t.Wait();
            }
        }
    }
}
