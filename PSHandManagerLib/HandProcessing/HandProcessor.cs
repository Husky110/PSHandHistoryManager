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
    public abstract class HandProcessor : IHandProcessor
    {
        protected string workingDirectory = ""; //
        protected string outputDirectory = ""; //
        protected bool storeProcessedHands = false;
        protected string storageDirectory = ""; //
        protected bool archiveUnprocessedHands = false;
        protected string archiveDirectory = "";//
        protected string fakePlayername = ""; //
        protected bool removeChatMessages = false;
        protected string outputFilename = "";
        protected HandTask handTask; //
        public Task attachedTask { get; set; } //
        protected Dictionary<String, String> languageDictionary;

        protected HandProcessor(HandTask handtaskToProcess)
        {
            if(ConfigurationManager.AppSettings["StoreHands"] == "1")
            {
                this.storageDirectory = ConfigurationManager.AppSettings["StorageFolder"];
            }
            if (ConfigurationManager.AppSettings["ArchiveUnprocessedHands"] == "1")
            {
                this.archiveDirectory = ConfigurationManager.AppSettings["ArchiveHandsFolder"];
            }
            
            this.workingDirectory = ConfigurationManager.AppSettings["WorkingDirectory"];
            this.outputDirectory = ConfigurationManager.AppSettings["OutputDirectory"];
            this.fakePlayername = ConfigurationManager.AppSettings["FakeplayerName"];
            this.removeChatMessages = Convert.ToBoolean(Convert.ToInt16(ConfigurationManager.AppSettings["RemoveChatMessages"]));
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

        protected abstract Dictionary<String, int> detectPlayers();

        public void processHandTask()
        {
            Dictionary<String, int> players = this.detectPlayers();
            bool taskSuccessfull = true;
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
                this.WriteHandFile(this.outputDirectory + this.outputFilename, newLines.ToArray());
            }
            else //if we have to do nothing...
            {
                this.WriteHandFile(this.outputDirectory + this.outputFilename, handTask.lines);
            }

            if (this.archiveUnprocessedHands)
            {
                string filePath = this.createDirectoriesForArchiveAndStorage(this.archiveDirectory) + this.outputFilename;
                this.WriteHandFile(filePath, this.handTask.lines);
            }

            if (this.storeProcessedHands)
            {
                string filePath = this.createDirectoriesForArchiveAndStorage(this.storageDirectory) + this.outputFilename;
                this.WriteHandFile(filePath, this.handTask.lines);
            }
            this.finishTask(taskSuccessfull);
        }

        protected string createDirectoriesForArchiveAndStorage(string sourcePath)
        {
            string currentDirPath = this.archiveDirectory + DateTime.Now.Year + "\\"; // add year to path
            checkDirectoryOrCreateIt(currentDirPath);
            currentDirPath += DateTime.Now.Month + "\\"; // add month
            checkDirectoryOrCreateIt(currentDirPath);
            currentDirPath += DateTime.Now.Day + "\\";
            checkDirectoryOrCreateIt(currentDirPath);
            currentDirPath += DateTime.Now.Hour + "\\";
            checkDirectoryOrCreateIt(currentDirPath);
            return currentDirPath;
        }

        protected void checkDirectoryOrCreateIt(string dirPath)
        {
            if (Directory.Exists(dirPath) == false)
            {
                Directory.CreateDirectory(dirPath);
            }
        }

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
