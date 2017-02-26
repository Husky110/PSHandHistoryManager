using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSHandManagerLib.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using System.IO;

namespace PSHandManagerLib.HandProcessing
{
    /// <summary>
    /// Baseclass for all HandProcessors.
    /// Provides the basic methods for handprocessing.
    /// </summary>
    public abstract class HandProcessor : IHandProcessor
    {
        /// <summary>
        /// Holds the path to the workingdirectory
        /// </summary>
        protected string workingDirectory = "";

        /// <summary>
        /// Holds the path to the outputdirectory
        /// </summary>
        protected string outputDirectory = "";

        /// <summary>
        /// Weither to store all processed hands or not
        /// </summary>
        protected bool storeProcessedHands = false;

        /// <summary>
        /// Holds the path to the storagedirectory
        /// </summary>
        protected string storageDirectory = "";

        /// <summary>
        /// Weither to archive the lines from before the processing
        /// </summary>
        protected bool archiveUnprocessedHands = false;

        /// <summary>
        /// Holds the path to the archive
        /// </summary>
        protected string archiveDirectory = "";

        /// <summary>
        /// Holds the fakeplayer-name
        /// </summary>
        protected string fakePlayername = "";

        /// <summary>
        /// The filename for the output/archive/storage
        /// </summary>
        protected string outputFilename = "";

        /// <summary>
        /// The HandTask which has to be processed
        /// </summary>
        protected HandTask handTask; //

        /// <summary>
        /// The attached Task-object
        /// </summary>
        public Task attachedTask { get; set; }

        /// <summary>
        /// The Dictionary for the language of the HandTask
        /// </summary>
        protected Dictionary<String, String> languageDictionary;

        /// <summary>
        /// Constructor
        /// Initializes everything
        /// </summary>
        /// <param name="handtaskToProcess">The HandTask which has to be processed</param>
        protected HandProcessor(HandTask handtaskToProcess)
        {
            if(ConfigurationManager.AppSettings["StoreHands"] == "1")
            {
                this.storeProcessedHands = true;
                this.storageDirectory = ConfigurationManager.AppSettings["StorageFolder"];
            }
            if (ConfigurationManager.AppSettings["ArchiveUnprocessedHands"] == "1")
            {
                this.archiveUnprocessedHands = true;
                this.archiveDirectory = ConfigurationManager.AppSettings["ArchiveHandsFolder"];
            }
            
            this.workingDirectory = ConfigurationManager.AppSettings["WorkingDirectory"];
            this.outputDirectory = ConfigurationManager.AppSettings["OutputDirectory"];
            this.fakePlayername = ConfigurationManager.AppSettings["FakeplayerName"];
            this.handTask = handtaskToProcess;
            this.outputFilename = this.handTask.handNum + ".txt";
            using (FileStream fs = new FileStream(Manager.appPath + "\\Localizations\\HandProcessing\\" + handtaskToProcess.handLanguage + ".json", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fs, new UTF8Encoding()))
                {
                   this.languageDictionary = JsonConvert.DeserializeObject<Dictionary<String, String>>(sr.ReadToEnd());
                }
            }
        }

        /// <summary>
        /// The must-override method to detect all players within the hand
        /// </summary>
        /// <returns>A Dictionary of players and what to do with them</returns>
        protected abstract Dictionary<String, int> detectPlayers();

        /// <summary>
        /// This is where the magic happens...
        /// This method is virtual, so any Language can override it.
        /// See the UnsupportedLanguageProcessor for an example
        /// </summary>
        public virtual void processHandTask()
        {

            Dictionary<String, int> players = this.detectPlayers();
            bool taskSuccessfull = true;
            string[] linesForFile; // basicly used for storage...
            if(players.Count > 0) //if there is a player
            {
                List<String> newLines = new List<string>();
                //add the first 2 Lines on default, since those are headerlines
                newLines.Add(handTask.lines[0]);
                newLines.Add(handTask.lines[1]);
                for(int x = 2; x < handTask.lines.Length; x++)
                {
                    string line = handTask.lines[x];
                    foreach(KeyValuePair<String, int> player in players)
                    {
                        if (line.Contains(player.Key))
                        {
                            switch (player.Value)
                            {
                                case 1:
                                    line = "";
                                    break;
                                case 2:
                                    /*
                                        TODO: this is a bit sloppy... there are combinations of a playername where this could replace more than it should.
                                        Example: playername - ": folds"
                                        But I wanna finish this somewhen and it shouldn't have that huge of an impact.
                                        So if you have a better idea on how to replace the playername within the line - implement it here :)
                                    */
                                    line = line.Replace(player.Key, this.fakePlayername);
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                            break;
                        }
                    }
                    if(line != "")
                    {
                        newLines.Add(line);
                    }
                }
                linesForFile = newLines.ToArray();
                this.WriteHandFile(this.outputDirectory + this.outputFilename, linesForFile);
            }
            else //if we have to do nothing...
            {
                linesForFile = handTask.lines;
                this.WriteHandFile(this.outputDirectory + this.outputFilename, linesForFile);
            }

            if (this.archiveUnprocessedHands)
            {
                string filePath = this.createDirectoriesForArchiveAndStorage(this.archiveDirectory) + this.outputFilename;
                this.WriteHandFile(filePath, this.handTask.lines);
            }

            if (this.storeProcessedHands)
            {
                string filePath = this.createDirectoriesForArchiveAndStorage(this.storageDirectory) + this.outputFilename;
                this.WriteHandFile(filePath, linesForFile);
            }
            this.finishTask(taskSuccessfull);
        }

        /// <summary>
        /// Creates the structure of the archive and storage
        /// </summary>
        /// <param name="sourcePath">Path like this: \Year\Monthnum\Daynum\Hournum</param>
        /// <returns></returns>
        protected string createDirectoriesForArchiveAndStorage(string sourcePath)
        {
            string currentDirPath = sourcePath + DateTime.Now.Year + "\\"; // add year to path
            checkDirectoryOrCreateIt(currentDirPath);
            currentDirPath += DateTime.Now.Month + "\\"; // add month
            checkDirectoryOrCreateIt(currentDirPath);
            currentDirPath += DateTime.Now.Day + "\\";
            checkDirectoryOrCreateIt(currentDirPath);
            currentDirPath += DateTime.Now.Hour + "\\";
            checkDirectoryOrCreateIt(currentDirPath);
            return currentDirPath;
        }

        /// <summary>
        /// Checks weither a directory exists or not and creates it if neccesary
        /// </summary>
        /// <param name="dirPath">Directory to check</param>
        protected void checkDirectoryOrCreateIt(string dirPath)
        {
            if (Directory.Exists(dirPath) == false)
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        /// <summary>
        /// Writes a file which contains a PS-hand
        /// </summary>
        /// <param name="filepath">Path to the file</param>
        /// <param name="linesToWrite">Lines which have to be written</param>
        protected void WriteHandFile(string filepath, string[] linesToWrite)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter sw = new StreamWriter(fs, new UTF8Encoding()))
                {
                    foreach (string line in linesToWrite)
                    {
                        sw.WriteLine(line);
                    }
                }
            }
        }

        /// <summary>
        /// Finalizes the task and marks itself as done
        /// </summary>
        /// <param name="sucessfull">Weither the task was sucessfull</param>
        protected void finishTask(bool sucessfull = false)
        {
            if (sucessfull)
            {
                File.Delete(this.workingDirectory + this.handTask.handNum + ".xml");
            }
            HandProcessDispatcher.currentRunningTasks--;
            bool ignoreOut;
            Manager.runningTasks.TryRemove(this.attachedTask, out ignoreOut);
        }
    }
}
