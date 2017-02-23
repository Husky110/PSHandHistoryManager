using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PSHandManagerLib.Tasks;
using PSHandManagerLib.Exceptions;

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
        /// The path to store all faulty files.
        /// Right now it's not in use.
        /// </summary>
        private string errorDirectory = ""; // TODO: right now this value is not in use, whereas it realy should be...

        /// <summary>
        /// Stores the Taskobject which runs this class.
        /// </summary>
        public Task attachedTask; //used to remove the Task from the FileSystemWatcher

        /// <summary>
        /// Is used to have a callback to the GUI in case that an old file is beeing imported.
        /// </summary>       
        public int detectedHands = 0; // TODO: create usage in GUI like "found x hands in file"

        /// <summary>
        /// Initialized the FileProcessor to start work
        /// </summary>
        /// <param name="filePath">Path to the PokerStars-file which has to be processed.</param>
        /// <param name="pathToWorkingDirectory">Path to the working-directory - where to output serialized hands.</param>
        public FileProcessor(string filePath, string pathToWorkingDirectory)
        {
            this.sourceFilePath = filePath;
            this.workingDirectory = pathToWorkingDirectory;
            this.errorDirectory = pathToWorkingDirectory + "\\faulty\\";
        }

        /// <summary>
        /// Processes the given PokerStars-file.
        /// </summary>
        public void processFile()
        {

            /*
                Normaly there should be only 1 Hand per file, since the FileSystemWatcher scans pretty fast.
                But to provide the functionality of scanning smaller old files (less than 10 MB) at once, we scan the whole file here.
                Additionally a file could have more than one hand if a files is beeing processed later on, since the Tasklimitation of the FileSystemWatcher.
            */
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
            int freespacecounter = 0;
            XmlSerializer serializer = new XmlSerializer(typeof(HandTask));
            List<String> handLines = new List<string>();

            foreach (String line in fileLines)
            {
                if (line != "") // normal Line
                {
                    handLines.Add(line);
                }
                else // every PS-Hand ends with 3 empty lines... we've got one here...
                {
                    freespacecounter++;
                }
                if (freespacecounter == 3) //create a new HandTask out of the previous lines, since we've found 3 empty lines
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
                    using (FileStream fs = new FileStream(this.workingDirectory + handnum + ".xml", FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        serializer.Serialize(fs, ht);
                    }
                    handLines = new List<string>();
                    freespacecounter = 0;
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
            }
            int ignoreoutFSW = 0; //just to be there... I ignore the out value actually...
            bool ignoreoutHP = false;
            FileSystemWatcher.files.TryRemove(this.sourceFilePath, out ignoreoutFSW);
            Manager.runningTasks.TryRemove(this.attachedTask, out ignoreoutHP);
            FileSystemWatcher.currentRunningTasks--;
        }
    }
}
