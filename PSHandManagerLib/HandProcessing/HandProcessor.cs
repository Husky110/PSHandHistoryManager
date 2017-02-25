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
        protected string storageDirectory = ""; //
        protected int storageMode = 0; // 0 = eine File pro Hand + pro Stunde ein Ordner, 1 = eine große File
        protected string archiveDirectory = "";//
        protected int archiveMode = 0; // 0 = eine File pro Hand + pro Stunde ein Ordner, 1 = eine große File
        protected string fakePlayername = ""; //
        protected bool removeChatMessages = false;
        protected DateTime now = DateTime.Now; //
        protected HandTask handTask; //
        public Task attachedTask { get; set; } //
        protected Dictionary<String, List<String>> languageDictionary;

        protected HandProcessor(HandTask handtaskToProcess)
        {
            if(ConfigurationManager.AppSettings["StoreHands"] == "1")
            {
                this.storageMode = Convert.ToInt16(ConfigurationManager.AppSettings["StorageMode"]);
                this.storageDirectory = ConfigurationManager.AppSettings["StorageFolder"];
            }
            if (ConfigurationManager.AppSettings["ArchiveUnprocessedHands"] == "1")
            {
                this.archiveMode = Convert.ToInt16(ConfigurationManager.AppSettings["ArchiveMode"]);
                this.archiveDirectory = ConfigurationManager.AppSettings["ArchiveHandsFolder"];
            }
            this.workingDirectory = ConfigurationManager.AppSettings["WorkingDirectory"];
            this.outputDirectory = ConfigurationManager.AppSettings["OutputDirectory"];
            this.fakePlayername = ConfigurationManager.AppSettings["FakeplayerName"];
            this.removeChatMessages = Convert.ToBoolean(ConfigurationManager.AppSettings["RemoveChatMessages"]);
            this.handTask = handtaskToProcess;
            using (FileStream fs = new FileStream(Manager.appPath + "\\Localizations\\English\\" + handtaskToProcess.handLanguage + ".json", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fs, new UTF8Encoding()))
                {
                   this.languageDictionary = JsonConvert.DeserializeObject<Dictionary<String, List<String>>>(sr.ReadToEnd());
                }
            }
        }

        public abstract void processHandTask();

        protected abstract List<String> detectPlayers();

    }
}
