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
using PSHandManagerLib.Exceptions;
using PSHandManagerLib.HandProcessing.LocalizedHandProcessors;

namespace PSHandManagerLib.HandProcessing
{
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
        /// Interval on how often the Dispatcher should look for new tasks to process
        /// </summary>
        private int interval = 1;

        public static ConcurrentQueue<HandTask> tasksToProcess = new ConcurrentQueue<HandTask>();

        public HandProcessDispatcher()
        {
            this.workingDirectory = ConfigurationManager.AppSettings["WorkingDirectory"];
            this.initializeHandProcessDispatcher();
        }

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

        public void run()
        {
            while(Manager.shutdown == false)
            {
                while(HandProcessDispatcher.tasksToProcess.Count > 0 && HandProcessDispatcher.currentRunningTasks < this.maxTasks)
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

                Thread.Sleep(this.interval * 1000);
            }
        }

        private IHandProcessor createHandProcessor(HandTask ht)
        {
            switch (ht.handLanguage)
            {
                case "English":
                    return new EnglishHandProcessor(ht);
                default:
                    throw ManagerException.createManagerException(300, new object[1] { ht.handLanguage }, new NotImplementedException());
            }
        }

    }
}
