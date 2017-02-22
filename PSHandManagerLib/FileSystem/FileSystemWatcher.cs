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
        private int interval = 1; //interval on how often the PSHandhistoryfolder is beeing scaned in seconds
        public static ConcurrentDictionary<String, bool> files = new ConcurrentDictionary<string, bool>(); //used to make sure that files are beeing scaned only once - used here and within the FileProcessor

        public FileSystemWatcher()
        {
            this.sourceFolder = ConfigurationManager.AppSettings["SourceFolder"];
            this.workingDirectory = ConfigurationManager.AppSettings["WorkingDirectory"];
            if (Directory.Exists(this.workingDirectory) == false)
            {
                Directory.CreateDirectory(this.workingDirectory);
            }
        }

        public void run()
        {
            while(HandProcessor.shutdown == false)
            {
                string[] foundFiles = Directory.GetFiles(this.sourceFolder);
                for(int x = 0; x < foundFiles.Length; x++)
                {
                    bool alreadyInProcess = false;
                    FileSystemWatcher.files.TryGetValue(foundFiles[x], out alreadyInProcess);
                    if(alreadyInProcess == false) {
                        FileProcessor fp = new FileProcessor(foundFiles[x], this.workingDirectory);
                        Task t = new Task(() => fp.processFile());
                        fp.attachedTask = t;
                        t.Start();
                        FileSystemWatcher.files.TryAdd(foundFiles[x], true);
                        HandProcessor.runningTasks.TryAdd(t, true);
                        
                    }
                }

                if(HandProcessor.shutdown == false) { 
                    Thread.Sleep(this.interval * 1000);
                }

            }

            //do operations to shutdown
        }

        
    }
}
