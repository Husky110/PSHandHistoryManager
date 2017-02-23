using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using System.Configuration;

namespace PSHandManagerLib.FileSystem
{
    class FileSystemWatcher
    {
        private string sourceFolder = "";
        private string workingDirectory = "";
        private int maxTasks = Environment.ProcessorCount;
        [ThreadStatic] public static int currentRunningTasks = 0;
        private int interval = 1; //interval on how often the PSHandhistoryfolder is beeing scaned in seconds
        public static ConcurrentDictionary<String, int> files = new ConcurrentDictionary<string, int>(); //used to make sure that files are beeing scaned only once - used here and within the FileProcessor

        public FileSystemWatcher()
        {
            this.sourceFolder = ConfigurationManager.AppSettings["SourceFolder"];
            this.workingDirectory = ConfigurationManager.AppSettings["WorkingDirectory"];
            if (Directory.Exists(this.workingDirectory) == false)
            {
                Directory.CreateDirectory(this.workingDirectory);
            }
            if(Directory.Exists(this.workingDirectory + "\\faulty") == false)
            {
                Directory.CreateDirectory(this.workingDirectory + "\\faulty");
            }
        }

        public void run()
        {
            while(Manager.shutdown == false)
            {
                string[] foundFiles = Directory.GetFiles(this.sourceFolder);
                for(int x = 0; x < foundFiles.Length; x++)
                {
                    if(FileSystemWatcher.currentRunningTasks < this.maxTasks)
                    { 
                        int ignoreout = 0;
                        bool alreadyInProcess = FileSystemWatcher.files.TryGetValue(foundFiles[x], out ignoreout); //check if we already know the file
                        if(alreadyInProcess == false) {
                            FileProcessor fp = new FileProcessor(foundFiles[x], this.workingDirectory);
                            Task t = new Task(() => fp.processFile());
                            fp.attachedTask = t;
                            t.Start();
                            FileSystemWatcher.currentRunningTasks++;
                            FileSystemWatcher.files.TryAdd(foundFiles[x], 1);
                            Manager.runningTasks.TryAdd(t, true);
                        }
                    }
                }

                if(Manager.shutdown == false) { 
                    Thread.Sleep(this.interval * 1000);
                }

            }

            //do operations to shutdown
        }

        
    }
}
