using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using PSHandManagerLib.Tasks;
using PSHandManagerLib.HandProcessing.LocalizedHandProcessors;

namespace PSHandManagerLib.HandProcessing
{
    /// <summary>
    /// This one dispatches all HandTasks which have to be processed.
    /// Start this always before the FileSystemWatcher
    /// </summary>
    public class HandProcessDispatcher
    {
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
        /// Interval on how often the Dispatcher should look for new tasks to process in milliseconds
        /// </summary>
        private int interval = 10;

        /// <summary>
        /// Holds all tasks which have to be processed.
        /// </summary>
        /// <remarks>
        /// This one is static so that a FileProcessor can add its created tasks itself and we don't have to watch the filesystem twice.
        /// </remarks>
        public static ConcurrentQueue<HandTask> tasksToProcess = new ConcurrentQueue<HandTask>();

        public HandProcessDispatcher()
        {
            this.workingDirectory = ConfigurationManager.AppSettings["WorkingDirectory"];
            this.initializeHandProcessDispatcher();
        }

        /// <summary>
        /// Checks the filesystem for any unprocessed hands from previous sessions
        /// </summary>
        private void initializeHandProcessDispatcher()
        {
            string[] serializedHandTasks = Directory.GetFiles(this.workingDirectory);
            XmlSerializer serializer = new XmlSerializer(typeof(HandTask));
            foreach(string serializedFile in serializedHandTasks)
            {
                using(FileStream fs = new FileStream(serializedFile, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    HandTask ht = (HandTask)serializer.Deserialize(fs);
                    HandProcessDispatcher.tasksToProcess.Enqueue(ht);
                }
            }
        }

        /// <summary>
        /// Dispatches all HandTasks
        /// </summary>
        public void run()
        {
            while(Manager.shutdown == false)
            {
                while (HandProcessDispatcher.tasksToProcess.Count > 0 && HandProcessDispatcher.currentRunningTasks < this.maxTasks)
                {
                    HandTask ht;
                    HandProcessDispatcher.tasksToProcess.TryDequeue(out ht);
                    IHandProcessor hp = this.createHandProcessor(ht);
                    Task t = new Task(() => hp.processHandTask());
                    hp.attachedTask = t;
                    HandProcessDispatcher.currentRunningTasks++;
                    Manager.runningTasks.TryAdd(t, true);
                    t.Start();
                }

                Thread.Sleep(this.interval);
            }
        }

        /// <summary>
        /// Creates a HandProcessor based on the language of the handtask
        /// </summary>
        /// <param name="ht">HandTask</param>
        /// <returns></returns>
        private IHandProcessor createHandProcessor(HandTask ht)
        {
            switch (ht.handLanguage)
            {
                case "English":
                    return new EnglishHandProcessor(ht);
                default:
                    return new UnsupportedLanguageHandProcessor(ht);
            }
        }

    }
}
