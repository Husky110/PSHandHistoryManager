using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PSHandManagerLib.Tasks;
using PSHandManagerLib.Exceptions;
using PSHandManagerLib.HandProcessing;
using PSHandManagerLib.Localizations;

namespace PSHandManagerLib.FileSystem
{
    /// <summary>
    /// Holds the logic for a single PokerStars-file which has to be processed.
    /// </summary>
    class FileProcessor
    {
        /// <summary>
        /// The path to the PokerStars-file which has to be processed.
        /// </summary>
        private string sourceFilePath = "";

        /// <summary>
        /// The path to the working-directory for the serialized hands.
        /// Stored here to avoid deadlocks on the Configuration.
        /// </summary>
        private string workingDirectory = "";

        /// <summary>
        /// Stores the Taskobject which runs this class.
        /// </summary>
        public Task attachedTask; //used to remove the Task from the FileSystemWatcher and the Manager

        /// <summary>
        /// Is used to have a callback to the GUI in case that an old file is beeing imported.
        /// </summary>       
        public int detectedHands = 0; // TODO: create usage in GUI like "found x hands in file"

        /// <summary>
        /// Used as a temporal storage for the created HandTasks to enque them in the HandProcessorDispatcher
        /// </summary>
        private List<HandTask> createdHandTasks = new List<HandTask>();

        /// <summary>
        /// Initialized the FileProcessor to start work
        /// </summary>
        /// <param name="filePath">Path to the PokerStars-file which has to be processed.</param>
        /// <param name="pathToWorkingDirectory">Path to the working-directory - where to output serialized hands.</param>
        public FileProcessor(string filePath, string pathToWorkingDirectory)
        {
            this.sourceFilePath = filePath;
            this.workingDirectory = pathToWorkingDirectory;
        }

        /// <summary>
        /// Processes the given PokerStars-file.
        /// </summary>
        /// <remarks>
        /// Normaly there should be only 1 Hand per file, since the FileSystemWatcher scans pretty fast.
        /// But to provide the functionality of scanning smaller old files(less than 10 MB) at once, we scan the whole file here.
        /// Additionally a file could have more than one hand if a files is beeing processed later on, since the Tasklimitation of the FileSystemWatcher.
        /// </remarks>
        public void processFile()
        {
            List<String> fileLines = new List<string>();
            try
            { 
                using (FileStream fs = new FileStream(this.sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        while (sr.Peek() > -1)
                        {
                            fileLines.Add(sr.ReadLine());
                        }
                    }
                }
            }
            catch(IOException ioEx) //mostlikely file is in use... try again 10 times and throw an exception then...
            {
                int attempts = 0;
                FileSystemWatcher.files.TryGetValue(this.sourceFilePath, out attempts);
                attempts++;
                if(attempts > 10)
                {
                    Manager.shutdown = true;
                    throw ManagerException.createManagerException(200,new object[1] { sourceFilePath }, ioEx);
                }
                else
                {
                    FileSystemWatcher.files.TryUpdate(this.sourceFilePath, attempts, attempts - 1);
                    Thread.Sleep(100);
                    this.processFile();
                    return;
                }

            }
            catch(Exception ex) //something bad goes on here for now we shutdown... TODO: better handling that doesn't crash the whole app...
            {
                Manager.shutdown = true;
                throw ex;
            }
            this.createHandTasks(fileLines);

            this.finishTask(true);
        }

        /// <summary>
        /// Creates HandTasks and serializes them for further processing.
        /// This method is used in normal processing, but can also be called directly from the GUI to process larger (more then 10 MB) old PokerStars-files.
        /// </summary>
        /// <param name="fileLines">A List of String containing one complete PokerStars-Hand</param>
        public void createHandTasks(List<String> fileLines)
        {
            bool lastLineWasFreeSpace = false;
            /*
                normaly a hands ends with 3 space-lines... 
                well an export of my HM2-Database showed that is is only true in 99.81% of all cases... (4 hands where missing out of 2083) 
                -> this was actually another bug in HM2 where the Database forgot the empty spaces after the hand.
                To even process this I use this slightly crude evaluation.
                TODO: implement a better solution to detect the beginning of a new hand... this one works for english and german, but might cause trouble in the future
            */
            XmlSerializer serializer = new XmlSerializer(typeof(HandTask));
            List<String> handLines = new List<string>();

            foreach (String line in fileLines)
            {
                if (line != "") // normal Line
                {
                    handLines.Add(line);
                    lastLineWasFreeSpace = false;
                }
                else
                {
                    if(lastLineWasFreeSpace == false)
                    {
                        this.detectedHands++;
                        HandTask ht = new HandTask();
                        ht.handSourceFilename = Path.GetFileNameWithoutExtension(this.sourceFilePath);
                        //Handnumberdetection
                        string handnum = handLines[0].Split(':')[0];
                        char[] handnumArray = handnum.ToCharArray();
                        for (int x = handnumArray.Length - 1; x > 0; x--)
                        {
                            if (Char.IsNumber(handnumArray[x]) == false && x > 0)
                            {
                                handnum = handnum.Substring(x + 1, handnum.Length - x - 1);
                                break;
                            }
                        }
                        ht.handNum = handnum;
                        ht.lines = new string[handLines.Count];
                        handLines.CopyTo(ht.lines);
                        ht.handLanguage = new HandLanguageDetector().detectHandLanguage(handLines, this.sourceFilePath, new DirectoryInfo(Path.GetDirectoryName(sourceFilePath)).Name);
                        List<String> supportedLanguages = new List<string>(Directory.GetFiles(Manager.appPath + "\\Localizations\\HandProcessing"));
                        using (FileStream fs = new FileStream(this.workingDirectory + handnum + ".xml", FileMode.CreateNew, FileAccess.Write, FileShare.None))
                        {
                            serializer.Serialize(fs, ht);
                        }
                        handLines = new List<string>();
                        this.createdHandTasks.Add(ht);
                    }
                    lastLineWasFreeSpace = true;
                }
            }
        }

        /// <summary>
        /// Finishes the current FileProcessor-Task and notifies the FileSystemWatcher and the Manager
        /// </summary>
        private void finishTask(bool taskSuccessful = false)
        {
            if (taskSuccessful)
            {
                File.Delete(this.sourceFilePath);
                foreach(HandTask ht in this.createdHandTasks)
                {
                    HandProcessDispatcher.tasksToProcess.Enqueue(ht);
                }
            }
            int ignoreoutFSW = 0; //just to be there... I ignore the out value actually...
            bool ignoreoutBool = false;
            FileSystemWatcher.files.TryRemove(this.sourceFilePath, out ignoreoutFSW);
            Manager.runningTasks.TryRemove(this.attachedTask, out ignoreoutBool);
            FileSystemWatcher.currentRunningTasks--;
        }
    }
}
