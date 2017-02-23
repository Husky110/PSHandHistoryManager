using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PSHandManagerLib.Tasks;

namespace PSHandManagerLib.FileSystem
{
    class FileProcessor
    {

        private string sourceFilePath = ""; //to avoid threadlocks on the Configuration
        private string workingDirectory = ""; //to avoid threadlocks on the Configuration
        private string errorDirectory = "";
        public Task attachedTask; //used to remove the Task from the FileSystemWatcher
           
        public int detectedHands = 0; // to have a callback to the GUI in case that an old file is beeing imported.

        public FileProcessor(string filePath, string pathToWorkingDirectory)
        {
            this.sourceFilePath = filePath;
            this.workingDirectory = pathToWorkingDirectory;
            this.errorDirectory = pathToWorkingDirectory + "\\faulty\\";
        }

        public void processFile()
        {

            //this way is provided to process files which contain more than 1 hand -> normaly there should be only 1 hand per file
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
                File.Delete(this.sourceFilePath);
            }
            catch(IOException ioEx) //mostlikely file is in use... try again 10 times and throw an exception then...
            {
                int attempts = 0;
                FileSystemWatcher.files.TryGetValue(this.sourceFilePath, out attempts);
                attempts++;
                if(attempts > 10)
                {
                    Manager.shutdown = true;
                    // TODO: make Exceptions localizeable
                    // TODO: better Exceptionhandling
                    throw new Exception("The file " + sourceFilePath +" can not be processed!" + Environment.NewLine + "Please close PokerStars and try again." + Environment.NewLine + "If that doesn't help, remove this file.",ioEx);

                }
                else
                {
                    FileSystemWatcher.files.TryUpdate(this.sourceFilePath, attempts, attempts - 1);
                    Thread.Sleep(100);
                    this.processFile();
                    return;
                }

            }
            catch(Exception ex) //something bad goes on here...
            {
                Manager.shutdown = true;
                throw ex;
            }
            this.createHandTasks(fileLines);

            this.finishTask();
        }

        /// <summary>
        /// Creates HandTasks and serializes them for further processing.
        /// This method can also be used to import old handfiles via the GUI where it's possible to split larger files into smaller chunks.
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
        private void finishTask()
        {
            int ignoreoutFSW = 0; //just to be there... I ignore the out value actually...
            bool ignoreoutHP = false;
            FileSystemWatcher.files.TryRemove(this.sourceFilePath, out ignoreoutFSW);
            Manager.runningTasks.TryRemove(this.attachedTask, out ignoreoutHP);
            FileSystemWatcher.currentRunningTasks--;
        }
    }
}
