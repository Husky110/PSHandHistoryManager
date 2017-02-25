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
    /// <summary>
    /// Scans the filesystem for new PokerStars-files and starts their processing.
    /// </summary>
    class FileSystemWatcher
    {
        /// <summary>
        /// Stores the path the the PokerStars-filesfolder.
        /// Defined here to avoid deadlocks on Configuration.
        /// </summary>
        private string sourceFolder = "";

        /// <summary>
        /// The path to the working-directory for the serialized hands.
        /// Stored here to avoid deadlocks on the Configuration.
        /// </summary>
        private string workingDirectory = "";

        /// <summary>
        /// Stores the number of logical CPU-Cores / 2 to have not more tasks than cores available.
        /// </summary>
        private int maxTasks = Environment.ProcessorCount / 2;

        /// <summary>
        /// Hold the number of current running Tasks. Is used for Tasklimitation and GUI callbacks.
        /// </summary>
        public static int currentRunningTasks = 0;

        /// <summary>
        /// Interval on how often the PSHandhistoryfolder is beeing scaned in seconds
        /// </summary>
        private int interval = 1;

        /// <summary>
        /// Is used to make sure that files are beeing scaned only once.
        /// </summary>
        public static ConcurrentDictionary<String, int> files = new ConcurrentDictionary<string, int>();

        /// <summary>
        /// Initializes the FileSystemWatcher.
        /// </summary>
        public FileSystemWatcher()
        {
            //TODO: this is a bit sloppy... should be moved to the setup...
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

        /// <summary>
        /// Runs the FileSystemWatcher.
        /// Scans for new PokerStars-files and starts their processing.
        /// </summary>
        public void run()
        {
            while(Manager.shutdown == false)
            {
                string[] foundFiles = Directory.GetFiles(this.sourceFolder);
                for(int x = 0; x < foundFiles.Length; x++)
                {
                    if (FileSystemWatcher.currentRunningTasks < this.maxTasks)
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
            // no shutdown-operations needed. the manager does that!
        }

        
    }
}
